# OpinionDataLoader

Sistema de Análisis de Opiniones de Clientes con Proceso ETL, desarrollado en C# / .NET 10 sobre SQL Server. El sistema consolida opiniones de clientes provenientes de encuestas internas, reseñas web y comentarios en redes sociales, cargándolas en una base de datos OLTP (`OpinionesOLTP`) mediante un pipeline ETL con validación en múltiples capas.

## Contenido

- [Descripción general](#descripción-general)
- [Arquitectura](#arquitectura)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Modelo de datos](#modelo-de-datos)
- [Stack tecnológico](#stack-tecnológico)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Flujo del ETL](#flujo-del-etl)
- [Vistas y consultas de reportería](#vistas-y-consultas-de-reportería)
- [Estado del proyecto](#estado-del-proyecto)

## Descripción general

El sistema extrae datos desde archivos CSV (encuestas internas, reseñas web, comentarios de redes sociales, catálogo de clientes y productos), los valida y transforma, y los carga en una base de datos relacional (`OpinionesOLTP`) mediante procedimientos almacenados. El diseño está pensado para quedar abierto a futuras fuentes de datos (API REST, Excel, web scraping) sin modificar la lógica de carga existente.

Principios de diseño aplicados:

- **Validación en múltiples capas**: el Service valida formato y resuelve claves foráneas antes de enviar los datos; el Stored Procedure valida integridad referencial y reglas de negocio antes de insertar. La base de datos se protege a sí misma independientemente de qué capa de aplicación la consuma.
- **Catálogos controlados**: tablas maestras (`RedSocial`, `TipoEncuesta`, `TipoClasificacion`) se siembran manualmente y nunca se auto-crean durante la carga.
- **Registros anónimos o inválidos se rechazan**, no se almacenan con valores nulos.
- **Carga por lotes (TVP)** para las tablas transaccionales de mayor volumen, evitando inserciones fila por fila.

## Arquitectura

```
CSV (fuente)
    │
    ▼
Service (extrae, valida formato, resuelve FK via DbContext de solo lectura)
    │
    ▼
DAO (transporta datos via ADO.NET puro)
    │
    ▼
Stored Procedure (valida integridad referencial, inserta, retorna contadores)
    │
    ▼
SQL Server (OpinionesOLTP)
```

- **Escrituras**: exclusivamente vía ADO.NET + Stored Procedures (no Entity Framework).
- **Lecturas**: Entity Framework Core (scaffolded con EF Core Power Tools) contra un `DbContext` de solo lectura, usado por los Services para resolver claves foráneas antes de insertar.

## Estructura del proyecto

```
OpinionDataLoader.Data/          Libreria de clases (DAOs, Services, Models, Result, Context)
├── Base/
│   └── OpinionDataConnection.cs    Clase base de conexion ADO.NET para los DAOs
├── Context/
│   ├── Configurations/              Configuraciones EF Core (auto-generadas)
│   └── OpinionesOLTPContext.cs      DbContext de solo lectura
├── DAOs/                            Acceso a datos via Stored Procedures
├── Interfaces/
│   ├── Daos/
│   └── Services/
├── Models/
│   ├── Csv/                         Clases de deserializacion CSV (CsvHelper)
│   └── dbo/                         Entidades EF (mapeo directo de tablas)
├── Result/                          OperationResult, OperationResultLoad, CodigoResultado
└── Services/                        Logica de extraccion, validacion y orquestacion de carga

OpinionDataLoader.Load/          Proyecto ejecutable (composition root)
├── Host/
│   └── AppHost.cs                   Configuracion de Host y DI
├── appsettings.json                 Connection string y rutas de CSV (no versionado)
└── Program.cs                       Punto de entrada, orquesta la ejecucion de los Services
```

## Modelo de datos

Base de datos `OpinionesOLTP`, con separación de filegroups: `PRIMARY` para catálogos/maestros, `Current` para tablas transaccionales.

**Catálogos y maestros**: `Categoria`, `Cliente`, `Producto`, `ProductoCategoria`, `RedSocial`, `TipoEncuesta`, `TipoClasificacion`, `FuenteDatos`.

**Transaccionales**: `WebReviews`, `SocialComments`, `Surveys`.

| Tabla | Fuente CSV | Patrón de carga |
|---|---|---|
| Categoria | products.csv (columna Categoría, deduplicada) | Fila por fila |
| Cliente | clients.csv | Fila por fila |
| Producto | products.csv | Fila por fila |
| WebReviews | web_reviews.csv | Batch (TVP) |
| SocialComments | social_comments.csv | Batch (TVP) |
| Surveys | surveys_part1.csv | Batch (TVP) |

Diagrama entidad-relación disponible en `Opiniones_DBA_OTLP_ER_drawio.pdf`.

## Stack tecnológico

- C# / .NET 10
- ADO.NET (`Microsoft.Data.SqlClient`) para escrituras
- Entity Framework Core (EF Core Power Tools scaffold) para lecturas
- CsvHelper para parsing de CSV
- SQL Server
- Microsoft.Extensions.Hosting / DependencyInjection

## Configuración

1. Ejecutar `SQLQuery1.sql` para crear la base de datos, tablas, filegroups y sembrar los catálogos controlados.
2. Ejecutar `ProceduresQuery.sql` para crear los Stored Procedures y Table-Valued Parameters (TVP).
3. Crear `OpinionDataLoader.Load/appsettings.json` (no está versionado por contener credenciales) con el siguiente formato:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "RutasCsv": {
    "Productos": "C:\\ruta",
    "Clientes": "C:\\ruta",
    "WebReviews": "C:\\ruta",
    "SocialComments": "C:\\ruta",
    "Surveys": "C:\\ruta"
  }
}
```

## Ejecución

El proyecto de inicio es `OpinionDataLoader.Load`. Al ejecutarlo, corre secuencialmente la carga de:

1. Categoria
2. Cliente
3. Producto
4. WebReview
5. SocialComment
6. Survey

Por cada tabla, se muestra en consola un resumen con: filas leídas del CSV, rechazadas antes de llegar al procedimiento (formato inválido, FK no resuelta, duplicado dentro del archivo), enviadas al procedimiento, insertadas, ya existentes y rechazadas por el procedimiento.

## Flujo del ETL

1. **Extracción**: cada Service lee su CSV correspondiente con CsvHelper.
2. **Validación de formato**: campos vacíos, fechas inválidas, duplicados dentro del mismo archivo se descartan antes de continuar.
3. **Resolución de claves foráneas**: `IdCliente`, `IdProducto`, `IdRedSocial`, `IdClasificacion`, `IdTipoEncuesta` se resuelven contra los catálogos vía el `DbContext` de solo lectura. Los registros que no resuelven se rechazan.
4. **Transporte**: el DAO recibe los datos ya resueltos (`Models/dbo`) y ejecuta el Stored Procedure correspondiente, vía parámetros individuales (catálogos) o Table-Valued Parameters (tablas transaccionales).
5. **Validación e inserción en base de datos**: el Stored Procedure valida integridad referencial, rangos y duplicados contra la tabla, e inserta. Retorna contadores de insertados, ya existentes y rechazados.

## Vistas y consultas de reportería

Vistas disponibles sobre la base analítica:

- `VW_OpinionesConsolidadas`: unifica WebReviews, SocialComments y Surveys en un solo conjunto con columnas comunes.
- `VW_TotalComentariosPorProducto`: conteo de comentarios procesados por producto, desglosado por fuente.
- `VW_ClasificacionOpiniones`: distribución de opiniones por clasificación (Positiva/Negativa/Neutra).
- `VW_SatisfaccionPorProducto`: promedio y porcentaje de satisfacción por producto.
- `VW_TendenciaSatisfaccionMensual`: evolución mensual de satisfacción por producto.

> Nota: `SocialComments` no cuenta con puntaje ni clasificación en el esquema actual (la clasificación automática de texto vía NLP queda fuera del alcance de esta fase). Las métricas de satisfacción y clasificación se calculan sobre `WebReviews` y `Surveys` únicamente.

## Estado del proyecto

- [x] Modelado de base de datos OLTP
- [x] Stored Procedures y TVP para carga de datos
- [x] Capa DAO (ADO.NET)
- [x] Capa Service (extracción, validación, resolución de FK)
- [x] Pipeline ETL ejecutable de extremo a extremo
- [x] Vistas y consultas de reportería
- [ ] Extensibilidad a otras fuentes (API REST, Excel)
- [ ] Diseño OLAP
- [ ] Dashboard interactivo

## Licencia

Proyecto académico, sin licencia de distribución definida.

# 63BITS Master Coding Standards

## 1. Architectural Patterns (Critical)
- **MVVM in MVC**: 
    - **Models**: Inherit `ModelBase`. Logic/Methods only. No properties.
    - **ViewModels**: Data/Properties only. No methods. Use `public record` with `get; init;`.
- **Repository Pattern**: Never instantiate directly. Use `RepositoryFactory` (which uses `DbContextFactory`).
- **Web Folders**: Models and Controllers must be in `Website` or `Admin` sub-namespaces.

## 2. Naming Conventions
- **Database**: Table names are PascalCase/Plural. Columns are `TableName + PropertyName` (e.g., `GiftTitle`).
- **Members**: Public members use `PascalCase`. Private members use `_underscoreCamelCase`.
- **Implicit Private**: Do not write the `private` keyword; it is implicit by 63BITS standards.

## 3. Style & Formatting (From Old File)
- **Local Variables**: Use `var`.
- **Braces**: Opening braces on a new line (Allman style).
- **Regions**: Classes MUST be organized into `#region Properties`, `#region Methods`, and `#region Nested Classes`.

## 4. Error Handling & Logging
- **The Library**: Use `SixtyThreeBitsDataObjectBase`.
- **The Wrapper**: All external operations (SQL, API, IO) must use `TryToReturn(() => { ... }, logString)`.
- **Log Formatting**: Use the helper format: `$"nameof(Method) (nameof(param) = {param})"`

## 5. Web & Routing
- **Routes**: Use Attribute Routing via `ControllerActionRouteNames`.
- **Views**: Use centralized `ViewNames`.
- **Frontend**: Use semantic HTML and consistent Razor patterns.
# Container State Pattern Demo

Este ejemplo demuestra el patrón **Container State** recomendado por Microsoft para la gestión eficiente del estado en aplicaciones Blazor.

## ¿Qué es el Container State Pattern?

El Container State Pattern es una técnica de gestión de estado que agrupa múltiples propiedades de estado relacionadas en un único objeto contenedor, en lugar de manejarlas como propiedades individuales.

## Problema que Resuelve

### Enfoque Tradicional (Problemático):
```csharp
public class FormService
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    // ... más propiedades individuales
    
    public event Action? OnChange;
    
    public void UpdateFirstName(string firstName)
    {
        FirstName = firstName;
        OnChange?.Invoke(); // Notificación individual
    }
    
    public void UpdateLastName(string lastName)
    {
        LastName = lastName;
        OnChange?.Invoke(); // Notificación individual
    }
    // ... más métodos con notificaciones individuales
}
```

**Problemas:**
- Múltiples notificaciones de cambio
- Estado disperso
- Validación fragmentada
- Lógica compleja de coordinación
- Sobrecarga de rendimiento

### Enfoque Container State (Solución):
```csharp
public class ContainerStateService
{
    private FormContainerState _formState = new();
    
    public event Action? OnChange;
    public FormContainerState FormState => _formState;
    
    public void UpdatePersonalInfo(string firstName, string lastName, string email)
    {
        _formState = _formState with
        {
            PersonalInfo = _formState.PersonalInfo with
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            }
        };
        OnChange?.Invoke(); // Una sola notificación para toda la actualización
    }
}

public record FormContainerState
{
    public PersonalInfo PersonalInfo { get; init; } = new();
    public Address Address { get; init; } = new();
    public UserPreferencesData Preferences { get; init; } = new();
    // ... estado agrupado lógicamente
}
```

## Beneficios del Container State

1. **Menos Notificaciones**: Una sola notificación por actualización de estado
2. **Estado Agrupado**: Las propiedades relacionadas se mantienen juntas
3. **Inmutabilidad**: Uso de records para estado inmutable
4. **Validación Centralizada**: Lógica de validación en el contenedor
5. **Mejor Rendimiento**: Menos actualizaciones de UI
6. **Mantenibilidad**: Código más limpio y organizado

## Características del Ejemplo

### Estructura del Estado
- **PersonalInfo**: Información personal del usuario
- **Address**: Datos de dirección
- **Preferences**: Configuraciones del usuario
- **Estados de UI**: Indicadores de carga y resultados

### Funcionalidades Demostradas
- Formulario multi-sección
- Validación en tiempo real
- Barra de progreso
- Manejo de estados de carga
- Resultados de envío
- Reset del formulario

### Patrón de Actualización
```csharp
// Actualización inmutable usando records
_formState = _formState with
{
    PersonalInfo = _formState.PersonalInfo with
    {
        FirstName = newFirstName
    }
};
```

## Comparación con Signals

Este ejemplo muestra el enfoque estándar de Blazor **sin** usar Signals, siguiendo las recomendaciones oficiales de Microsoft. La gestión de estado se realiza mediante:

- Servicios Scoped registrados en DI
- Eventos `OnChange` para notificaciones
- Records inmutables para el estado
- Patrón with-expressions para actualizaciones

## Cuándo Usar Container State

✅ **Usar cuando:**
- Tienes múltiples propiedades de estado relacionadas
- Necesitas validación coordinada
- Quieres reducir notificaciones de cambio
- El estado tiene una estructura lógica clara

❌ **No usar cuando:**
- Solo tienes 1-2 propiedades simples
- El estado no está relacionado lógicamente
- Necesitas actualizaciones muy granulares

## Referencias

- [Microsoft Docs: Blazor State Management](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management)
- [Container State Pattern](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management?view=aspnetcore-9.0#container-state)
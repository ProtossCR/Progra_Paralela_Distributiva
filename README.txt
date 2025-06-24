Simulación de Parque de Atracciones en C#

Objetivo

Simular el comportamiento de visitantes accediendo a diferentes atracciones en un parque, asegurando la sincronización adecuada con mecanismos de concurrencia (`lock` y `Monitor.TryEnter`) para evitar condiciones de carrera, sobrecupo y conflictos de acceso.

---

Implementación

El proyecto se desarrolló completamente en **C# 7.3**, compatible con Visual Studio, evitando el uso de características modernas como `new()` con tipo de destino.

Se usaron las siguientes clases:

`Atraccion`
- Representa una atracción como la montaña rusa, noria o carrusel.
- Tiene una **capacidad máxima**.
- Controla el acceso con:
  - `Monitor.TryEnter(...)`: los visitantes intentan ingresar durante un tiempo limitado (500 ms).
  - `lock`: para sincronizar operaciones seguras como aumentar o disminuir el número de ocupantes.
- Si está llena o no se logra entrar, el visitante intenta otra atracción o espera.

`Visitante`
- Representa a un visitante que corre en un **hilo independiente**.
- Intenta subirse 5 veces a atracciones aleatorias.
- Usa `Thread.Sleep()` para simular tiempos de espera y recorrido entre atracciones.

`Program`
- Punto de entrada.
- Crea 3 atracciones con diferentes capacidades.
- Lanza 10 hilos (`Thread`), cada uno asociado a un visitante.
- Espera que todos terminen con `Join()`.

---

Decisiones de Diseño

- Se usó **Monitor.TryEnter** en lugar de un `lock` directo para:
  - Evitar bloqueos indefinidos cuando una atracción está ocupada.
  - Permitir reintentos o cambio de atracción si está llena.
- Se controló la capacidad de forma segura usando `_ocupando` y verificaciones protegidas por `lock` y `Monitor`.
- Se usaron **objetos compartidos y protegidos** entre múltiples hilos para simular la concurrencia real.

---

¿Cómo se probó?

- Al ejecutar, se observa en consola:
  - Visitantes intentando acceder a atracciones.
  - Mensajes que indican si pudieron subirse, si la atracción estaba llena o si falló el intento de adquirir el lock.
  - Subida y bajada sincronizada sin superarse la capacidad definida.
  - Salida limpia de todos los hilos y cierre del programa.

**Ejemplo de salida esperada:**

Visitante 1 quiere subirse a Noria...
→ Visitante 1 se sube a Noria (1/4)
Visitante 2 quiere subirse a Noria...
→ Visitante 2 se sube a Noria (2/4)
...
← Visitante 1 baja de Noria (1/4)
← Visitante 2 baja de Noria (0/4)
[Visitante 1] Ha terminado su recorrido.


---

Requisitos técnicos

- .NET Framework compatible con C# 7.3 o superior.
- Visual Studio o entorno similar para ejecución y depuración.
- No requiere librerías externas.

---

Referencias

Microsoft. (s.f.). *Monitor.TryEnter(Object, Int32) Method*. Microsoft Learn.  
https://learn.microsoft.com/es-es/dotnet/api/system.threading.monitor.tryenter

Microsoft. (s.f.). *lock statement (C# reference)*. Microsoft Learn.  
https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/statements/lock

---
Autor

Sendel MJ  
Fecha de entrega: 24/06/2025
Curso: Programación Paralela y Distributiva

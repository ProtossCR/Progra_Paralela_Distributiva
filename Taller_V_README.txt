# Simulación de Procesamiento de Pedidos Multicliente en C#

## 🎯 Objetivo

Implementar un backend simulado donde múltiples clientes (productores) generan pedidos simultáneamente y varios operarios (consumidores) los procesan, garantizando integridad y sin bloqueos manuales.

---

## 🛠️ Tecnología y herramientas

- **Lenguaje**: C# 7.3  
- **Colección concurrente**: `BlockingCollection<Pedido>`  
- **Conteo atómico**: `Interlocked.Increment`  
- **Entorno**: Visual Studio (Consola .NET)

---

## 🧩 Diseño de la solución

1. **Estructura de datos**  
   - `struct Pedido { int Id; string Cliente; }`  
2. **Colección compartida**  
   - `BlockingCollection<Pedido>` como buffer seguro entre productores y consumidores.  
3. **Productores**  
   - Dos tareas (`Task.Run`) que generan cada una 5 pedidos, con IDs únicos y breve `Thread.Sleep` para simular actividad.  
4. **Consumidores**  
   - Dos tareas que recorren `GetConsumingEnumerable()` y procesan pedidos hasta que se agote la cola.  
5. **Contador atómico**  
   - Uso de `Interlocked.Increment(ref _pedidosProcesados)` para contar sin locks adicionales.  
6. **Terminación limpia**  
   - Al finalizar los productores, se llama a `_colaPedidos.CompleteAdding()` para que los consumidores concluya el bucle.

---

## 🔒 Justificación de concurrencia sin locks manuales

- **BlockingCollection** maneja internamente sincronización y notifica automáticamente a consumidores cuando hay ítems.  
- **Interlocked** ofrece operaciones atómicas muy eficientes para contadores simples, evitando zonas críticas manuales.  
- Evitamos:
  - Condiciones de carrera en el acceso a la cola.  
  - Necesidad de `lock`/`Monitor` alrededor de código de encolado/contador.  
  - Posibles bloqueos (deadlocks) o errores por abandono de locks.

---

## 🧪 Pruebas y funcionamiento

1. Ejecuta la aplicación.  
2. Observa en consola:
   - Mensajes de creación de pedidos por cada productor.  
   - Mensajes de procesamiento por cada consumidor e incremento atómico del contador.  
3. Al terminar, verás el total de pedidos procesados (debe coincidir con la suma de ambos productores).  

---

**Ejemplo de salida**
[Productor:ClienteA] Creó Pedido 1
[Productor:ClienteB] Creó Pedido 6
...
[Consumidor:Operario1] Procesando Pedido 1 de ClienteA...
[Consumidor:Operario1] Pedido 1 procesado. Contador actualizado: 1
...
Todos los pedidos han sido procesados. Total procesados: 10

---

## 📚 Referencias

Microsoft. (s.f.). *BlockingCollection<T> Class*. Microsoft Learn.  
https://learn.microsoft.com/es-es/dotnet/api/system.collections.concurrent.blockingcollection-1

Microsoft. (s.f.). *Interlocked.Increment Method*. Microsoft Learn.  
https://learn.microsoft.com/es-es/dotnet/api/system.threading.interlocked.increment

---

## ✍️ Autor

Sendel Madrigal
Fecha de entrega: 01/07/2025
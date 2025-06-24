using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Taller_IV
{
    // ---------------------------------------------------
    // Clase que modela una atracción con capacidad limitada
    // ---------------------------------------------------
    public class Atraccion
    {
        public string Nombre { get; }
        private readonly int _capacidad;           // Capacidad máxima de la atracción
        private int _ocupando = 0;                 // Cuántos visitantes están dentro
        private readonly object _lockObj = new object();  // Objeto para Monitor/lock

        public Atraccion(string nombre, int capacidad)
        {
            Nombre = nombre;
            _capacidad = capacidad;
        }

        // Método que simula subir, disfrutar y bajar de la atracción
        // Usa Monitor.TryEnter para intentar entrar con timeout
        public void SubirYDisfrutar(int idVisitante)
        {
            Console.WriteLine($"Visitante {idVisitante} quiere subirse a {Nombre}...");

            // Intentamos adquirir el lock durante hasta 500 ms
            if (Monitor.TryEnter(_lockObj, 500))
            {
                try
                {
                    // Dentro del lock comprobamos si hay espacio
                    if (_ocupando < _capacidad)
                    {
                        _ocupando++; // Reservamos un lugar
                        Console.WriteLine($"  → Visitante {idVisitante} se sube a {Nombre} " +
                                          $"({_ocupando}/{_capacidad}).");
                    }
                    else
                    {
                        // Atracción llena
                        Console.WriteLine($"  × {Nombre} está llena para Visitante {idVisitante}.");
                        return;
                    }
                }
                finally
                {
                    // Liberamos el lock de la sección de conteo
                    Monitor.Exit(_lockObj);
                }

                // Simulamos el tiempo de disfrute de la atracción
                Thread.Sleep(new Random().Next(800, 1500));

                // Al bajar, volvemos a lock para decrementar el contador
                lock (_lockObj)
                {
                    _ocupando--;
                    Console.WriteLine($"  ← Visitante {idVisitante} baja de {Nombre} " +
                                      $"({_ocupando}/{_capacidad}).");
                }
            }
            else
            {
                // No consiguió el lock en el timeout
                Console.WriteLine($"  ! Visitante {idVisitante} no pudo entrar al lock de {Nombre}.");
            }
        }
    }

    // ---------------------------------------------------
    // Clase que representa al visitante (cada uno corre en su propio hilo)
    // ---------------------------------------------------
    public class Visitante
    {
        private readonly int _id;
        private readonly List<Atraccion> _atracciones;
        private readonly Random _rnd = new Random();  // Inicialización clásica

        public Visitante(int id, List<Atraccion> atracciones)
        {
            _id = id;
            _atracciones = atracciones;
        }

        // Método que ejecutará cada hilo: intenta varias veces subirse a atracciones aleatorias
        public void RecorrerParque()
        {
            for (int intento = 0; intento < 5; intento++)
            {
                // Escoge una atracción aleatoria
                var atr = _atracciones[_rnd.Next(_atracciones.Count)];
                atr.SubirYDisfrutar(_id);

                // Espera un poco antes del siguiente intento (simula recorrer el parque)
                Thread.Sleep(_rnd.Next(300, 800));
            }

            Console.WriteLine($"[Visitante {_id}] Ha terminado su recorrido.\n");
        }
    }

    // ---------------------------------------------------
    // Punto de entrada: crea atracciones, visitantes e hilos
    // ---------------------------------------------------
    class Program
    {
        static void Main()
        {
            // 1. Configuramos el parque con 3 atracciones de distinta capacidad
            List<Atraccion> atracciones = new List<Atraccion>
            {
                new Atraccion("Montaña Rusa", 2),
                new Atraccion("Noria", 4),
                new Atraccion("Carrusel", 3)
            };

            // 2. Creamos y arrancamos 10 hilos, cada uno con un Visitante distinto
            List<Thread> hilos = new List<Thread>();
            for (int i = 1; i <= 10; i++)
            {
                Visitante visitante = new Visitante(i, atracciones);
                Thread hilo = new Thread(visitante.RecorrerParque)
                {
                    Name = $"Hilo-Visitante-{i}"
                };
                hilos.Add(hilo);
                hilo.Start();
            }

            // 3. Esperamos a que todos los visitantes terminen su recorrido
            foreach (Thread hilo in hilos)
            {
                hilo.Join();
            }

            Console.WriteLine("=== Todos los visitantes han salido del parque ===");
            Console.ReadLine();
        }
    }
}

using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence
{
    public class SubastaDbContext : DbContext
    {
        public SubastaDbContext(DbContextOptions<SubastaDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Billetera> Billeteras { get; set; }
        public DbSet<Subasta> Subastas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Puja> Pujas { get; set; }
        public DbSet<Transaccion_Ledger> Transacciones_Ledger { get; set; }
        public DbSet<Auditoria_Log> Auditorias_Log { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*Claves primarias*/
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Billetera>().HasKey(b => b.Id);
            modelBuilder.Entity<Subasta>().HasKey(s => s.Id);
            modelBuilder.Entity<Categoria>().HasKey(c => c.Id);
            modelBuilder.Entity<Puja>().HasKey(p => p.Id);
            modelBuilder.Entity<Transaccion_Ledger>().HasKey(t => t.Id);
            modelBuilder.Entity<Auditoria_Log>().HasKey(a => a.Id);

            //Usuario 1:1 Billetera
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Billetera)
                .WithOne(b => b.Usuario)
                .HasForeignKey<Billetera>(b => b.Usuario_Id)
                .OnDelete(DeleteBehavior.Cascade);

            //Usuario (Vendedor) 1:N Subasta
            modelBuilder.Entity<Subasta>()
                .HasOne(s => s.Vendedor)
                .WithMany(u => u.Subastas)
                .HasForeignKey(s => s.Vendedor_Id)
                .OnDelete(DeleteBehavior.Restrict); 

            // Usuario (Comprador) 1:N Puja
            modelBuilder.Entity<Puja>()
                .HasOne(p => p.Comprador)
                .WithMany(u => u.Pujas)
                .HasForeignKey(p => p.Comprador_Id)
                .OnDelete(DeleteBehavior.Restrict); 

            // Categoria 1:N Subasta
            modelBuilder.Entity<Subasta>()
                .HasOne(s => s.Categoria)
                .WithMany(c => c.Subastas)
                .HasForeignKey(s => s.Categoria_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Subasta 1:N Puja
            modelBuilder.Entity<Puja>()
                .HasOne(p => p.Subasta)
                .WithMany(s => s.Pujas)
                .HasForeignKey(p => p.Subasta_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Billetera 1:N Transaccion_Ledger
            modelBuilder.Entity<Transaccion_Ledger>()
                .HasOne(t => t.Billetera)
                .WithMany(b => b.Transacciones)
                .HasForeignKey(t => t.Billetera_Id)
                .OnDelete(DeleteBehavior.Cascade);


            //relaciones opcionales
            // Subasta 1:N Transaccion_Ledger (Opcional - Trazabilidad)
            modelBuilder.Entity<Transaccion_Ledger>()
                .HasOne(t => t.Subasta)
                .WithMany(s => s.Transacciones)
                .HasForeignKey(t => t.Subasta_Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Usuario 1:N Auditoria_Log (Opcional - Null si fue el Worker)
            modelBuilder.Entity<Auditoria_Log>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Auditorias)
                .HasForeignKey(a => a.Usuario_Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            
            //optimistic lockinh
            modelBuilder.Entity<Billetera>()
                .Property(b => b.Version)
                .IsRowVersion();

            modelBuilder.Entity<Subasta>()
                .Property(s => s.Version)
                .IsRowVersion();


            // Índices para optimizar volumen de consultas
            modelBuilder.Entity<Subasta>()
                .HasIndex(s => new { s.Estado, s.Categoria_Id, s.Fecha_Fin })
                .IncludeProperties(s => new { s.Titulo, s.Precio_Base });

            modelBuilder.Entity<Subasta>()
                .HasIndex(s => s.Fecha_Fin)
                .HasFilter("[Estado] = 'ACTIVA'");

            modelBuilder.Entity<Puja>()
                .HasIndex(p => new { p.Subasta_Id, p.Monto });

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();


            //configuración de tipo decimales
            modelBuilder.Entity<Subasta>().Property(s => s.Precio_Base).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Subasta>().Property(s => s.Incremento_Minimo).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Billetera>().Property(b => b.Saldo_Total).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Billetera>().Property(b => b.Saldo_Retenido).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Billetera>().Property(b => b.Saldo_Disponible).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Puja>().Property(p => p.Monto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Transaccion_Ledger>().Property(t => t.Monto).HasColumnType("decimal(18,2)");


            DateTime fechaBase = DateTime.Now;
            string hash = "$2y$10$TodzyNai9KReU03EQu46Y.OG28bJbBSupc2PT8BXgeTbINfVWEJuK"; // Hash de "123456"

            // 1. Usuarios
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Email = "vendedor@test.com", Nombre = "Vendedor", Password_Hash = hash, Fecha_Registro = fechaBase.AddDays(-30) },
                new Usuario { Id = 2, Email = "comprador1@test.com", Nombre = "Comprador Lider", Password_Hash = hash, Fecha_Registro = fechaBase.AddDays(-20) },
                new Usuario { Id = 3, Email = "comprador2@test.com", Nombre = "Comprador Habilitado", Password_Hash = hash, Fecha_Registro = fechaBase.AddDays(-15) },
                new Usuario { Id = 4, Email = "sinfondos@test.com", Nombre = "Usuario Sin Fondos", Password_Hash = hash, Fecha_Registro = fechaBase.AddDays(-10) }
            );

            // 2. Billeteras
            modelBuilder.Entity<Billetera>().HasData(
                new Billetera { Id = 1, Usuario_Id = 1, Saldo_Total = 0, Saldo_Retenido = 0, Saldo_Disponible = 0 },
                new Billetera { Id = 2, Usuario_Id = 2, Saldo_Total = 150000, Saldo_Retenido = 45000, Saldo_Disponible = 105000 },
                new Billetera { Id = 3, Usuario_Id = 3, Saldo_Total = 200000, Saldo_Retenido = 0, Saldo_Disponible = 200000 },
                new Billetera { Id = 4, Usuario_Id = 4, Saldo_Total = 500, Saldo_Retenido = 0, Saldo_Disponible = 500 }
            );

            // 3. Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Tecnologia" },
                new Categoria { Id = 2, Nombre = "Coleccionables" },
                new Categoria { Id = 3, Nombre = "Indumentaria" },
                new Categoria { Id = 4, Nombre = "Vehiculos" }
            );

            // 4. Subastas (Requisitos de cátedra + pruebas de fin de semana)
            modelBuilder.Entity<Subasta>().HasData(
                // 1. Activa estándar: Cierra en 25 min (con 2 pujas previas)
                new Subasta { Id = 1, Titulo = "MacBook Pro", Descripcion = "Activa Estandar", Url_Imagen = "https://imgs.search.brave.com/J6-wF1GTePlaf-q8ScW9AG59Iiwp5IWXjXc42fJ1rqg/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly93d3cu/YXBwbGUuY29tL3Yv/bWFjYm9vay1wcm8v/YXgvaW1hZ2VzL292/ZXJ2aWV3L3dlbGNv/bWUvaGVyb19lbmRm/cmFtZV9fZndldjll/Ymg0Mm1xX3hsYXJn/ZS5qcGc", Categoria_Id = 1, Vendedor_Id = 1, Precio_Base = 10000, Incremento_Minimo = 500, Fecha_Inicio = fechaBase.AddHours(-1), Fecha_Fin = fechaBase.AddMinutes(25), Estado = "ACTIVA" },

                // 2. Activa crítica: Cierra en 1 min (para probar anti-sniping)
                new Subasta { Id = 2, Titulo = "Reloj Antiguo", Descripcion = "Activa Crítica", Url_Imagen = "https://imgs.search.brave.com/wLx1K_lbAiBugpCVvNHXj4bv3w48P3iTTWjqGKbuNNg/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9pLmVi/YXlpbWcuY29tL2lt/YWdlcy9nL3U0d0FB/T1N3YW5SWGcxYlEv/cy1sNDAwLndlYnA", Categoria_Id = 2, Vendedor_Id = 1, Precio_Base = 5000, Incremento_Minimo = 100, Fecha_Inicio = fechaBase.AddHours(-1), Fecha_Fin = fechaBase.AddMinutes(2), Estado = "ACTIVA" },

                // 3. Próxima: Programada a +24 hs (pujas bloqueadas)
                new Subasta { Id = 3, Titulo = "Campera Cuero", Descripcion = "Próxima", Url_Imagen = "https://imgs.search.brave.com/78zXlb1RFASO9v8TmGTyEHbjuQV3Z4Pb2jr9Y1A6mlA/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9odHRw/Mi5tbHN0YXRpYy5j/b20vRF9OUV9OUF82/NDA3NTEtTUxBNTEz/NDY3MjgyMTZfMDgy/MDIyLVcud2VicA", Categoria_Id = 3, Vendedor_Id = 1, Precio_Base = 20000, Incremento_Minimo = 1000, Fecha_Inicio = fechaBase.AddHours(24), Fecha_Fin = fechaBase.AddHours(48), Estado = "PROGRAMADA" },

                // 4. Vencida con ganador
                new Subasta { Id = 4, Titulo = "Moto Honda", Descripcion = "Vencida con ganador", Url_Imagen = "https://imgs.search.brave.com/OwjFjSvv0S8ohAmt395Fx2GCJChEk7U13LAuRYJt_lU/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9zY2Fs/ZXRodW1iLmxlcGFy/a2luZy5mci91bnNh/ZmUvMzAweDIyNS9o/dHRwczovL2Nsb3Vk/LmxlcGFya2luZy1t/b3RvLmZyLzIwMjUv/MTAvMTAvMDcvMTEv/aG9uZGEtbmVzLW1v/dG9ycmFkLWhvbmRh/LW5lcy0xMjVfMjUz/OTU0NDY4LmpwZw", Categoria_Id = 4, Vendedor_Id = 1, Precio_Base = 100000, Incremento_Minimo = 5000, Fecha_Inicio = fechaBase.AddDays(-5), Fecha_Fin = fechaBase.AddDays(-1), Estado = "FINALIZADA" },

                // 5. Vencida desierta
                new Subasta { Id = 5, Titulo = "Monitor 4K", Descripcion = "Vencida desierta", Url_Imagen = "https://imgs.search.brave.com/GxxLh9QfgiHzyAdfGJdwmlCY3HDs81brY9Rr58XEfGI/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9tLm1l/ZGlhLWFtYXpvbi5j/b20vaW1hZ2VzL0kv/NjFlWFdSeGd4Skwu/anBn", Categoria_Id = 1, Vendedor_Id = 1, Precio_Base = 30000, Incremento_Minimo = 1000, Fecha_Inicio = fechaBase.AddDays(-4), Fecha_Fin = fechaBase.AddDays(-2), Estado = "DESIERTA" },

                // 6. Activa que vence el SÁBADO 19/09/2026
                new Subasta { Id = 6, Titulo = "Smart TV Samsung 55'", Descripcion = "Vence el Sábado para probar pujas", Url_Imagen = "https://imgs.search.brave.com/jJ3FtIEDfioe0Qx2YFJLbqkKmZJ_xxLZQjm6zW1P3b0/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9tLm1l/ZGlhLWFtYXpvbi5j/b20vaW1hZ2VzL0kv/OTFibFNpckVqT0wu/anBn", Categoria_Id = 1, Vendedor_Id = 1, Precio_Base = 80000, Incremento_Minimo = 2000, Fecha_Inicio = fechaBase, Fecha_Fin = fechaBase.AddDays(4), Estado = "ACTIVA" },

                // 7. Activa que vence el DOMINGO 20/09/2026
                new Subasta { Id = 7, Titulo = "PlayStation 5", Descripcion = "Vence el Domingo para probar pujas", Url_Imagen = "https://imgs.search.brave.com/TZ19Apjni8mGzUZkWey-zYtFy4VZ6lDLhLxrWhSQesQ/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly93d3cu/ZW5nYWRnZXQuY29t/L2ltZy9nYWxsZXJ5/L2hvdy1sb25nLWNh/bi15b3UtZXhwZWN0/LWEtcGxheXN0YXRp/b24tNS10by1sYXN0/L2ludHJvLTE3ODUx/NTQ5MDUuanBn", Categoria_Id = 1, Vendedor_Id = 1, Precio_Base = 120000, Incremento_Minimo = 3000, Fecha_Inicio = fechaBase, Fecha_Fin = fechaBase.AddDays(5), Estado = "ACTIVA" }
            );

            // 5. Pujas previas
            modelBuilder.Entity<Puja>().HasData(
                new Puja { Id = 1, Subasta_Id = 1, Comprador_Id = 3, Monto = 25000, Fecha_Puja = fechaBase.AddMinutes(-40) },
                new Puja { Id = 2, Subasta_Id = 1, Comprador_Id = 2, Monto = 45000, Fecha_Puja = fechaBase.AddMinutes(-10) }, // Líder $45.000
                new Puja { Id = 3, Subasta_Id = 4, Comprador_Id = 3, Monto = 120000, Fecha_Puja = fechaBase.AddDays(-2) }
            );

            // 6. Transacciones (Ledger)
            modelBuilder.Entity<Transaccion_Ledger>().HasData(
                new Transaccion_Ledger { Id = 1, Billetera_Id = 2, Tipo = "DEPOSITO", Monto = 150000, Fecha = fechaBase.AddDays(-5) },
                new Transaccion_Ledger { Id = 2, Billetera_Id = 3, Tipo = "DEPOSITO", Monto = 200000, Fecha = fechaBase.AddDays(-5) },
                new Transaccion_Ledger { Id = 3, Billetera_Id = 4, Tipo = "DEPOSITO", Monto = 500, Fecha = fechaBase.AddDays(-5) },
                new Transaccion_Ledger { Id = 4, Billetera_Id = 2, Subasta_Id = 1, Tipo = "RETENCION", Monto = 45000, Fecha = fechaBase.AddMinutes(-10) }, // Retención $45.000 Comprador 1
                new Transaccion_Ledger { Id = 5, Billetera_Id = 3, Subasta_Id = 4, Tipo = "DEBITO", Monto = 120000, Fecha = fechaBase.AddDays(-1) }
            );
        }
    }
}
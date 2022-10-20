using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TritechPruebaLeo.Models;
using System.Text.Json;

namespace TritechPruebaLeo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IConfiguration Configuration { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Productos()
        {
            ViewBag.Productos = GetProductos();

            return View();
        }

      


        //*Crud Productos
        public List<producto> GetProductos()
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var List_Productos = new List<producto>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_getProducto";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = command.ExecuteReader();
                   
                    while (reader.Read())
                    {
                        producto producto = new producto();
                        producto.noParte = Convert.ToString(reader["noParte"]);
                        producto.nombre = Convert.ToString(reader["nombre"]);
                        producto.descripcion = Convert.ToString(reader["descripcion"]);
                        producto.precio = Convert.ToDecimal(reader["precio"]);
                        producto.cantidad = Convert.ToInt32(reader["cantidad"]);
                        producto.json = "";
                        List_Productos.Add(producto);
                    }
                    connection.Close();
                }

            }
                return List_Productos;
        }


        [HttpPost]
         public ResultDto setProducto(producto producto)
        {
            ResultDto dto=new ResultDto();
            dto.estatus= 0;
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var List_Productos = new List<producto>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_Producto_save";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("noParte", producto.noParte);
                    command.Parameters.AddWithValue("nombre", producto.nombre);
                    command.Parameters.AddWithValue("descripcion", producto.descripcion);
                    command.Parameters.AddWithValue("precio", producto.precio);
                    command.Parameters.AddWithValue("cantidad", producto.cantidad);
                    try
                    {
                        dto.estatus = command.ExecuteNonQuery();
                        dto.msj = "Agregado";
                    }
                    catch (Exception e)
                    {
                        connection.Close();

                        dto.estatus = 0;
                        dto.msj = e.Message;
                    }


                    connection.Close();
                }

            }



            return dto;
        }



          public IActionResult Movimiento()
        {

            ViewBag.Movimiento = GetMovimiento();
            ViewBag.Productos = GetProductos();
            ViewBag.conten = GetContenedores();
            return View();
        }


        //*Crud Movimientos

        public List<movimiento> GetMovimiento()
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var List_movimiento = new List<movimiento>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_getMovimiento";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        movimiento movimiento= new movimiento();
                        movimiento.id = Convert.ToInt32(reader["id"]);
                        movimiento.tipo = Convert.ToString(reader["tipo"]);
                        movimiento.estatus = Convert.ToString(reader["estatus"]);
                        List_movimiento.Add(movimiento);
                    }
                    connection.Close();
                }

            }
            return List_movimiento;
        }


        [HttpPost]
        public ResultDto setMovimiento(movimiento movimiento)
        {
            if (movimiento.id.Equals(null))
            {
                movimiento.id = 0;
            }

            ResultDto dto = new ResultDto();
            dto.estatus = 0;
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_movimiento_save";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", movimiento.id);
                    command.Parameters.AddWithValue("tipo", movimiento.tipo);
                    command.Parameters.AddWithValue("estatus", movimiento.estatus);

                    try
                    {
                        dto.estatus = command.ExecuteNonQuery();
                        dto.msj = "Agregado";
                    }
                    catch (Exception e)
                    {
                        connection.Close();

                        dto.estatus = 0;
                        dto.msj = e.Message;
                    }


                    connection.Close();
                }

            }



            return dto;
        }


        [HttpPost]
        public ResultDto afectaMovimiento(int id)
        {
            ResultDto dto = new ResultDto();
            dto.estatus = 0;
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_Afectar";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    try
                    {
                        dto.estatus = command.ExecuteNonQuery();
                        dto.msj = "Afectado";
                    }
                    catch (Exception e)
                    {
                        connection.Close();

                        dto.estatus = 0;
                        dto.msj = e.Message;
                    }


                    connection.Close();
                }

            }



            return dto;
        }

        //*Crud contenedores


        public List<contenedor> GetContenedores()
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var lista = new List<contenedor>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_getContenedores";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        contenedor contenedor = new contenedor();
                        contenedor.id = Convert.ToInt32(reader["id"]);
                        contenedor.tipo = Convert.ToString(reader["tipo"]);
                        lista.Add(contenedor);
                    }
                    connection.Close();
                }

            }
            return lista;
        }


        //* Crud MovimientoDetalle

        [HttpPost]
        public ResultDto setDetalle(movimientoDetalle md)
        {
            ResultDto dto = new ResultDto();
            dto.estatus = 0;
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_saveDetalle";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id_movimiento", md.id_movimiento);
                    command.Parameters.AddWithValue("noParte_producto", md.noParte_producto);
                    command.Parameters.AddWithValue("id_contenedor", md.id_contenedor);
                    command.Parameters.AddWithValue("piezasXContenedor", md.piezasXContenedor);
                    command.Parameters.AddWithValue("contenedorXPallet", md.contenedorXPallet);
                    try
                    {
                        dto.estatus = command.ExecuteNonQuery();
                        dto.msj = "Agregado";
                    }
                    catch (Exception e)
                    {
                        connection.Close();

                        dto.estatus = 0;
                        dto.msj = e.Message;
                    }


                    connection.Close();
                }

            }



            return dto;
        }

        public List<movimientoDetalle> GetDetalle(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var List_movimiento = new List<movimientoDetalle>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_getDetalle";
                    command.Parameters.AddWithValue("Idmovimiento", id);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        movimientoDetalle movimientoDetalle = new movimientoDetalle();
                        movimientoDetalle.id = Convert.ToInt32(reader["id"]);
                        movimientoDetalle.id_movimiento = Convert.ToInt32(reader["id_movimiento"]);
                        movimientoDetalle.noParte_producto = Convert.ToString(reader["noParte_producto"]);
                        movimientoDetalle.id_contenedor = Convert.ToInt32(reader["id_contenedor"]);
                        movimientoDetalle.piezasXContenedor = Convert.ToInt32(reader["piezasXContenedor"]);
                        movimientoDetalle.contenedorXPallet = Convert.ToInt32(reader["contenedorXPallet"]);
                        movimientoDetalle.TotalPiezas = Convert.ToInt32(reader["TotalPiezas"]);
                        movimientoDetalle.contenedor = new contenedor();

                        movimientoDetalle.contenedor.tipo = Convert.ToString(reader["tipoContenedor"]);
                        List_movimiento.Add(movimientoDetalle);
                    }
                    connection.Close();
                }

            }
            return List_movimiento;
        }

        [HttpPost]
        public ResultDto deleteDetalle(int id)
        {
            ResultDto dto = new ResultDto();
            dto.estatus = 0;
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_deleteDetalle";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    try
                    {
                        dto.estatus = command.ExecuteNonQuery();
                        dto.msj = "Eliminado";
                    }
                    catch (Exception e)
                    {
                        connection.Close();

                        dto.estatus = 0;
                        dto.msj = e.Message;
                    }


                    connection.Close();
                }

            }



            return dto;
        }

    }
}
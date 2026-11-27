using Nextcar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Nextcar.Controllers
{
    public class HomeController : Controller
    {
        private const string Conexao = "Data Source=concessionaria.db";

        
        public IActionResult Index()
        {
            var lista = new List<Carro>();
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            using var cmd = new SqliteCommand("select * from carro order by id desc", cone);
            using var l = cmd.ExecuteReader();
            while (l.Read())
                lista.Add(Ler(l));
            return View(lista);
        }

       
        public IActionResult Detalhes(int id)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            using var cmd = new SqliteCommand("select * from carro where id = @id", cone);
            cmd.Parameters.AddWithValue("@id", id);
            using var l = cmd.ExecuteReader();
            if (!l.Read()) return NotFound();
            return View(Ler(l));
        }

        
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Carro c)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            var sql = @"insert into carro (marca, modelo, ano, preco, quilometragem, cor, descricao, imagem)
                        values (@marca, @modelo, @ano, @preco, @km, @cor, @desc, @img)";
            using var cmd = new SqliteCommand(sql, cone);
            cmd.Parameters.AddWithValue("@marca", c.Marca ?? "");
            cmd.Parameters.AddWithValue("@modelo", c.Modelo ?? "");
            cmd.Parameters.AddWithValue("@ano", c.Ano);
            cmd.Parameters.AddWithValue("@preco", c.Preco);
            cmd.Parameters.AddWithValue("@km", c.Quilometragem);
            cmd.Parameters.AddWithValue("@cor", c.Cor ?? "");
            cmd.Parameters.AddWithValue("@desc", c.Descricao ?? "");
            cmd.Parameters.AddWithValue("@img", ValidarImagem(c.Imagem));
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

       
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            using var cmd = new SqliteCommand("select * from carro where id = @id", cone);
            cmd.Parameters.AddWithValue("@id", id);
            using var l = cmd.ExecuteReader();
            if (!l.Read()) return NotFound();
            return View(Ler(l));
        }

        [HttpPost]
        public IActionResult Editar(int id, Carro c)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            var sql = @"update carro set
                            marca = @marca, modelo = @modelo, ano = @ano, preco = @preco,
                            quilometragem = @km, cor = @cor, descricao = @desc, imagem = @img
                        where id = @id";
            using var cmd = new SqliteCommand(sql, cone);
            cmd.Parameters.AddWithValue("@marca", c.Marca ?? "");
            cmd.Parameters.AddWithValue("@modelo", c.Modelo ?? "");
            cmd.Parameters.AddWithValue("@ano", c.Ano);
            cmd.Parameters.AddWithValue("@preco", c.Preco);
            cmd.Parameters.AddWithValue("@km", c.Quilometragem);
            cmd.Parameters.AddWithValue("@cor", c.Cor ?? "");
            cmd.Parameters.AddWithValue("@desc", c.Descricao ?? "");
            cmd.Parameters.AddWithValue("@img", ValidarImagem(c.Imagem));
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Detalhes", new { id });
        }

        
        [HttpPost]
        public IActionResult Excluir(int id)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            using var cmd = new SqliteCommand("delete from carro where id = @id", cone);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

                private static string ValidarImagem(string? url)
        {
            if (Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri.ToString();
            }
            return "";
        }

        private static string Texto(SqliteDataReader l, string coluna)
        {
            int i = l.GetOrdinal(coluna);
            return l.IsDBNull(i) ? "" : l.GetString(i);
        }

        private static Carro Ler(SqliteDataReader l)
        {
            return new Carro
            {
                Id = l.GetInt32(l.GetOrdinal("id")),
                Marca = l.GetString(l.GetOrdinal("marca")),
                Modelo = l.GetString(l.GetOrdinal("modelo")),
                Ano = l.GetInt32(l.GetOrdinal("ano")),
                Preco = l.GetDecimal(l.GetOrdinal("preco")),
                Quilometragem = l.GetInt32(l.GetOrdinal("quilometragem")),
                Cor = Texto(l, "cor"),
                Descricao = Texto(l, "descricao"),
                Imagem = Texto(l, "imagem")
            };
        }
    }
}
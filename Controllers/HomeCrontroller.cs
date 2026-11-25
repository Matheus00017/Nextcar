using Concessionaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Concessionaria.Controllers
{
    public class HomeController : Controller
    {
        private const string Conexao = "Data Source=concessionaria.db";

        // Lista todos os carros
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

        // Detalhes de um carro
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

        // Mostra o formulário
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        // Recebe o formulário e grava
        [HttpPost]
        public IActionResult Cadastrar(Carro c)
        {
            using var cone = new SqliteConnection(Conexao);
            cone.Open();
            var sql = @"insert into carro (marca, modelo, ano, preco, quilometragem, cor, descricao)
                        values (@marca, @modelo, @ano, @preco, @km, @cor, @desc)";
            using var cmd = new SqliteCommand(sql, cone);
            cmd.Parameters.AddWithValue("@marca", c.Marca ?? "");
            cmd.Parameters.AddWithValue("@modelo", c.Modelo ?? "");
            cmd.Parameters.AddWithValue("@ano", c.Ano);
            cmd.Parameters.AddWithValue("@preco", c.Preco);
            cmd.Parameters.AddWithValue("@km", c.Quilometragem);
            cmd.Parameters.AddWithValue("@cor", c.Cor ?? "");
            cmd.Parameters.AddWithValue("@desc", c.Descricao ?? "");
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
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
                Cor = l.IsDBNull(l.GetOrdinal("cor")) ? "" : l.GetString(l.GetOrdinal("cor")),
                Descricao = l.IsDBNull(l.GetOrdinal("descricao")) ? "" : l.GetString(l.GetOrdinal("descricao"))
            };
        }
    }
}
using CrossScripting.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CrossScripting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProdutosDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var consulta = "SELECT Id, Nome, Descricao FROM Produtos WHERE Id = @id;";

            try
            {
                using (var conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    using (SqlCommand comando = new SqlCommand(consulta, conexao))
                    {
                        comando.CommandText = consulta;
                        comando.Parameters.Add(new SqlParameter("@id", id));

                        var leitor = comando.ExecuteReader();
                        if (leitor.HasRows)
                        {
                            while (leitor.Read())
                            {
                                ViewBag.Id = int.Parse(leitor["Id"].ToString());
                                ViewBag.Nome = leitor["Nome"].ToString();
                                ViewBag.Descricao = leitor["Descricao"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Mensagem = "Erro: " + e.Message;
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, string nome, string descricao)
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProdutosDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var consulta = "UPDATE Produtos SET Nome = @nome, Descricao = @descricao WHERE Id = @id;";

            try
            {
                using (var conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    using (SqlCommand comando = new SqlCommand(consulta, conexao))
                    {
                        comando.CommandText = consulta;

                        comando.Parameters.Add(new SqlParameter("@id", id));
                        comando.Parameters.Add(new SqlParameter("@nome", RemoverTagsHtml(nome)));
                        comando.Parameters.Add(new SqlParameter("@descricao", RemoverTagsHtml(descricao)));

                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Mensagem = "Erro: " + e.Message;
            }

            return RedirectToAction("List");
        }

        // Função que remove as tags HTML para evitar CrossScripting
        private string RemoverTagsHtml(string texto)
        {
            var result = Regex.Replace(texto, @"<[^>]*>", String.Empty);

            return result;
        }

        [HttpGet]
        public ActionResult List(int? id)
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProdutosDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var consulta = "SELECT Id, Nome, Descricao FROM Produtos";

            try
            {
                using (var conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    using (SqlCommand comando = new SqlCommand(consulta, conexao))
                    {
                        if (id.HasValue)
                        {
                            comando.CommandText = consulta + " WHERE Id = @id;";
                            comando.Parameters.Add(new SqlParameter("@id", id));
                        }

                        var leitor = comando.ExecuteReader();
                        if (leitor.HasRows)
                        {
                            var produtos = new List<Produto>();
                            while (leitor.Read())
                            {
                                produtos.Add(new Produto
                                {
                                    Id = int.Parse(leitor["Id"].ToString()),
                                    Nome = leitor["Nome"].ToString(),
                                    Descricao = leitor["Descricao"].ToString()
                                }
                                );
                            }
                            ViewBag.Produtos = produtos;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Mensagem = "Erro: " + e.Message;
            }

            return View();
        }
    }
}
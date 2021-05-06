using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static readonly int StartId = 4;
        private static readonly int EndId = 13;

        private static readonly string fileName = "posts.txt";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Получение данных о постах с ID [4; 13] и запись результата в файл");
            
            var posts = await GetAllPosts();
            var resultNumber = 0;
            
            try
            {
                using StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
                foreach (var post in posts)
                {
                    var postData = $"{post.UserId}\n{post.Id}\n{post.Title}\n{post.Body}\n";
                    await sw.WriteLineAsync(postData);
                    resultNumber++;
                }
            }
            catch
            {
                Console.WriteLine("Ошибка записи в файл!");
                throw;
            }

            Console.WriteLine($"Запись выполнена успешно! Количество записей {resultNumber}");

            Console.ReadKey();
        }

        private static async Task<List<Post>> GetAllPosts()
        {
            var tasks = new List<Task<Post>>();

            for (int i = StartId; i <= EndId; i++)
            {
                tasks.Add(GetPostById(i));
            }

            var allTasks = Task.WhenAll(tasks);

            try
            {
                await allTasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return tasks.Where(t => t.IsFaulted == false).Select(t => t.Result).ToList();
        }

        private static async Task<Post> GetPostById(int id)
        {
            try
            {
                var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var post = Newtonsoft.Json.JsonConvert.DeserializeObject<Post>(responseContent);

                return post;
            }
            catch
            {
                Console.WriteLine($"Возникла ошибка при получении данных о посте с ID {id}");
                throw;
            }
        }
    }
}

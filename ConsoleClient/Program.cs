
using Fruit; 

var client = new FruitClient("https://localhost:7186", new HttpClient()); 
Fruit.Fruit created = await client.FruitPOSTAsync("123", new Fruit.Fruit { Name = "Banana", Stock = 100 }); 
Console.WriteLine($"Created {created.Name}");
Fruit.Fruit fetched = await client.FruitGETAsync("123"); 
Console.WriteLine($"Fetched {fetched.Name}");

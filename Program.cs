using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var cone = new SqliteConnection("Data Source=concessionaria.db"))
{
    cone.Open();
    var sql = @"create table if not exists carro (
        id integer primary key autoincrement,
        marca text not null,
        modelo text not null,
        ano integer not null,
        preco real not null,
        quilometragem integer not null default 0,
        cor text,
        descricao text,
        imagem text
    )";
    new SqliteCommand(sql, cone).ExecuteNonQuery();
}

app.Run();
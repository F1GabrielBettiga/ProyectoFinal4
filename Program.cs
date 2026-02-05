using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProyectoFinal4.Data;
using ProyectoFinal4.Models;
using ProyectoFinal4.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Incluir DbContext
builder.Services.AddDbContext<MovieDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//agregamos identity core y no utilizamo la identity uai porque no necesitamos todas las funcionalidades pero sigue siendo identity
//“Quiero usar Identity, pero solo lo básico (usuarios, passwords, roles, login), sin toda la UI ni configuraciones automáticas.”
/*Lo que NO te da
❌ Autenticación configurada
❌ Cookies
❌ Scheme por defecto
❌ Login persistente
*/

builder.Services.AddIdentityCore<Usuario>(options =>
{
    // Opciones de contraseña básicas
    options.Password.RequireDigit = false; //sirve para que no pida digito
    options.Password.RequireLowercase = false; //sirve para que no pida minuscula
    options.Password.RequireUppercase = false;//sirve para que no pida mayuscula
    options.Password.RequireNonAlphanumeric = false; //sirve para que no pida caracter especial
    options.Password.RequiredLength = 3; //longitud minima de la contraseña
    options.SignIn.RequireConfirmedAccount = false; //no requiere confirmacion de cuenta para iniciar sesion

})
.AddRoles<IdentityRole>() //agrega el soporte para roles
.AddEntityFrameworkStores<MovieDbContext>()//indica que usaremos EF para almacenar los datos de identidad
.AddSignInManager();//agrega el servicio de SignInManager para manejar el inicio de sesion


//Manejo de cookie, Lo ponemos en default, pero hay que ponerlo ya que identity core no lo pone automaticamente y ASP.NET necesita una forma de recordar quién es en cada request
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
})
.AddIdentityCookies();


//Ruteos. Este bloque configura cómo se comporta la cookie de Identity y a dónde manda al usuario según la situación:
builder.Services.ConfigureApplicationCookie(options =>
{ 
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //tiempo de expiracion de la cookie, 60 minutos sin actividad → sesión muere
    options.SlidingExpiration = true; //renovar la cookie con cada request
    options.LoginPath = "/Account/Login"; //ruta personalizada para el login
    options.AccessDeniedPath = "/Account/AccessDenied"; //ruta personalizada para acceso denegado
}); //si no colocamos estas rutas, usaria las por defecto de Identity que no tenemos porque no usamos la UI de Identity y no las encotraria 


//Servicios de archivos (Imganenes en este caso)
builder.Services.AddScoped<ImagenStorage>(); 
builder.Services.Configure<FormOptions>(o => { o.MultipartBodyLengthLimit = 2 * 1024 * 1024; }); //limite de 2 MB para las imagenes



var app = builder.Build();

//invocar la ejecucion del dbseeder con un using scope, con un try catch
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MovieDbContext>();
        await DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); //tiene que estar antes de authorization
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

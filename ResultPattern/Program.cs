using ResultPattern.Models;
using ResultPattern.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Role> roles = [new Role() { Name = "Admin" }, new Role() { Name = "Root" }, new Role() { Name = "Client" }];

app.MapGet("roles", () =>
{

    List<RoleViewModel> rolesViewModel = [];

    foreach (var role in roles)
    {
        rolesViewModel.Add(new()
        {
            Id = role.Id,
            Name = role.Name,
        });
    }

    ResponseHelper response = new()
    {
        Result = rolesViewModel
    };

    return Results.Ok(response);
})
    .WithName("ObtenerRoles")
    .WithDescription("Este endpoint regresa una lista de roles de tipo RoleViewModel")
    .WithTags("Roles")
    //.WithMetadata([new Microsoft.AspNetCore.Authorization.AuthorizeAttribute()])
    .Produces(StatusCodes.Status200OK)
    .WithOpenApi();

app.MapGet("roles/{id}", (string id) =>
{
    ResponseHelper response = new();

    if (string.IsNullOrEmpty(id))
    {
        response.Message = "El parametro id es requerido.";

        return Results.BadRequest(response);
    }

    var role = roles.Find(x => x.Id == id);

    if (role is null)
    {
        response.Message = "El role no ha sido encontrado.";

        return Results.NotFound(response);
    }

    RoleViewModel roleViewModel = new() { Id = role.Id, Name = role.Name };

    response.IsSuccess = true;

    response.Result = roleViewModel;

    return Results.Ok(response);

})
    .WithName("ObtenerRolePorId")
    .WithDescription("Este endpoint espera un parametro de tipo string y regresa un role de tipo RoleViewModel")
    .WithTags("Roles")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .WithOpenApi();

app.MapPost("roles", (RoleDTO roleDTO) =>
{
    ResponseHelper response = new();

    if (string.IsNullOrEmpty(roleDTO.Name))
    {
        response.Message = "El nombre no puede ser vacío.";
        return Results.BadRequest(response);
    }

    var roleExists = roles.Any(x => x.Name.ToLower() == roleDTO.Name.ToLower());

    if (roleExists)
    {
        response.Message = "El role ya existe, elige otro nombre";
        return Results.BadRequest(response);
    }

    roles.Add(new Role() { Name = roleDTO.Name });

    response.IsSuccess = true;

    response.Message = "Role creado correctamente";

    return Results.Created("", response);
});


app.MapPut("roles/{id}", (string id, RoleDTO roleDTO) =>
{
    ResponseHelper response = new();

    if (string.IsNullOrEmpty(id))
    {
        response.Message = "El role no ha sido encontrado.";
        return Results.NotFound(response);
    }

    var role = roles.Where(x => x.Id == id).FirstOrDefault();

    if (role is not null)
    {
        var roleExists = roles.Any(x => x.Name.ToLower() == roleDTO.Name.ToLower());

        if (roleExists)
        {
            response.Message = "El role ya existe, elige otro nombre";
            return Results.BadRequest(response);
        }
    }

    role.Name = roleDTO.Name;

    response.IsSuccess = true;

    response.Message = "El role ha sido actualizado con exito.";

    return Results.Ok(response);
});

app.MapDelete("roles/{id}", (string id) =>
{
    ResponseHelper response = new();

    if (string.IsNullOrEmpty(id))
    {
        response.Message = "El role no ha sido encontrado.";
        return Results.NotFound(response);
    }

    var role = roles.Where(x => x.Id == id).FirstOrDefault();

    if (role is null)
    {
        response.Message = "El role no ha sido encontrado.";

        return Results.NotFound(response);
    }

    roles.Remove(role);

    response.IsSuccess = true;

    response.Message = "El role ha sido borrado con exito";

    return Results.Ok(response);
});

app.UseCors();

app.Run();


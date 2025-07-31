using Microsoft.Data.SqlClient;
using System.Data;
using Domain.Entities;

namespace Infrastructure.Data;

public class UsuariosDbContext
{
    private readonly string _connectionString;

    public UsuariosDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<IM253E03Usuario> List()
    {
        var data = new List<IM253E03Usuario>();
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT [Id],[Nombre],[Direccion],[Telefono],[Correo] FROM [IM253E03Usuario]", con);
        con.Open();
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            data.Add(new IM253E03Usuario
            {
                Id = (Guid)dr["Id"],
                Nombre = (string)dr["Nombre"],
                Direccion = dr["Direccion"] as string,
                Telefono = (string)dr["Telefono"],
                Correo = (string)dr["Correo"]
            });
        }
        return data;
    }

    public IM253E03Usuario? Details(Guid id)
    {
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT [Id],[Nombre],[Direccion],[Telefono],[Correo] FROM [IM253E03Usuario] WHERE [Id]=@id", con);
        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
        con.Open();
        var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return new IM253E03Usuario
            {
                Id = (Guid)dr["Id"],
                Nombre = (string)dr["Nombre"],
                Direccion = dr["Direccion"] as string,
                Telefono = (string)dr["Telefono"],
                Correo = (string)dr["Correo"]
            };
        }
        return null;
    }

    public void Create(IM253E03Usuario usuario)
    {
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("INSERT INTO [IM253E03Usuario] ([Id],[Nombre],[Direccion],[Telefono],[Correo]) VALUES (@id,@nombre,@direccion,@telefono,@correo)", con);
        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = usuario.Id == Guid.Empty ? Guid.NewGuid() : usuario.Id;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 256).Value = usuario.Nombre;
        cmd.Parameters.Add("@direccion", SqlDbType.NVarChar).Value = (object?)usuario.Direccion ?? DBNull.Value;
        cmd.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = usuario.Telefono;
        cmd.Parameters.Add("@correo", SqlDbType.NVarChar).Value = usuario.Correo;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public void Edit(IM253E03Usuario usuario)
    {
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("UPDATE [IM253E03Usuario] SET [Nombre]=@nombre,[Direccion]=@direccion,[Telefono]=@telefono,[Correo]=@correo WHERE [Id]=@id", con);
        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = usuario.Id;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 256).Value = usuario.Nombre;
        cmd.Parameters.Add("@direccion", SqlDbType.NVarChar).Value = (object?)usuario.Direccion ?? DBNull.Value;
        cmd.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = usuario.Telefono;
        cmd.Parameters.Add("@correo", SqlDbType.NVarChar).Value = usuario.Correo;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(Guid id)
    {
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("DELETE FROM [IM253E03Usuario] WHERE [Id]=@id", con);
        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
        con.Open();
        cmd.ExecuteNonQuery();
    }
}

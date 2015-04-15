using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Data;

using System;
using TransportDepot.Models.DataValidation;
namespace TransportDepot.DataValidation
{
  public class DataValidationService : IDataValidationService
  {
    private string _connectionString;

    
    public Models.DataValidation.ValidationResponse Validate(string name)
    {
      var responses = this.ValidateMany(new string[] { name });
      if (responses.Count() == 0)
      {
        return new ValidationResponse
        {
          Name = "Invalid Request",
          Success = false,
          Text = "No validation was possible"
        };
      }
      return responses.First();
    }

    public IEnumerable<ValidationResponse> ValidateMany(IEnumerable<string> names)
    {
      var validators = this.GetValidators(names);
      var responses = validators.Select(v => new ValidationResponse
      {
        Name = v.Name,
        Success   = v.IsValid,
        Text = v.Message
      }).ToList();
      return responses;
    }

    private IEnumerable<IValidator> GetValidators(IEnumerable<string> names)
    {
      var validators = FindValidators(names);
      validators.ToList().ForEach(v =>
        {
          v.Validate();
        });
      return validators;
    }

    private IEnumerable<IValidator> FindValidators(IEnumerable<string> names)
    {
      var namesXmlDoc = new XDocument(new XElement("validators",
        names.Select(n => new XElement("validator", n))));

      var query = @"
          DECLARE @x XML
          SET @x = @ValidatorsXmlString
          ;WITH [ValidatorNames] AS
          (
            SELECT [T].[c].value('.', 'varchar(50)') AS [Name]
            FROM @x.nodes('//validator') AS T(C)
          )

          SELECT [V].[Name]
               , [V].[Class]
          FROM [dbo].[Validator] [V]
          INNER JOIN [ValidatorNames] [N]
            ON [V].[Name] = [N].[Name]
          ";
      var validatorNamesTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(query))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@ValidatorsXmlString", namesXmlDoc.ToString());
        adapter.Fill(validatorNamesTable);
      }
      var validators = validatorNamesTable.AsEnumerable().Select(v =>
        {
          var validatorName = v.Field<string>("Class");
          IValidator validator = Activator.CreateInstance("TransportDepot.DataValidation.Validators", validatorName) as IValidator;
          validator.Name = validatorName;
          return validator;
        });
      return validators;
    }


    private string ConnectionString
    {
      get 
      {
        if (string.IsNullOrEmpty(this._connectionString))
        {
          this._connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        }
        return this._connectionString;
      }
    }
  }
}

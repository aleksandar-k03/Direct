using Direct.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Direct
{
  public abstract partial class DirectDatabaseBase : IDisposable
  {

    ///
    /// Load int
    ///

    public virtual int? LoadInt(string query, params object[] parameters) { return this.LoadInt(this.Construct(query, parameters)); }
    public virtual int? LoadInt(string command)
      => this.Loader.LoadSingle<int>(command);
    public virtual async Task<int?> LoadIntAsync(string query, params object[] parameters) => await LoadIntAsync(this.Construct(query, parameters));
    public virtual async Task<int?> LoadIntAsync(string command)
      => await this.Loader.LoadSingleAsync<int?>(command);

    ///
    /// Load double
    ///

    public virtual double? LoadDouble(string query, params object[] parameters) { return this.LoadDouble(this.Construct(query, parameters)); }
    public virtual double? LoadDouble(string command)
      => this.Loader.LoadSingle<double?>(command);
    public virtual async Task<double?> LoadDoubleAsync(string query, params object[] parameters) => await LoadDoubleAsync(this.Construct(query, parameters));
    public virtual async Task<double?> LoadDoubleAsync(string command)
      => await this.Loader.LoadSingleAsync<double?>(command);

    ///
    /// Load bool
    ///

    public virtual bool? LoadBool(string query, params object[] parameters) { return this.LoadBool(this.Construct(query, parameters)); }
    public virtual bool? LoadBool(string command)
      => this.Loader.LoadSingle<bool?>(command);
    public virtual async Task<bool?> LoadBoolAsync(string query, params object[] parameters) => await LoadBoolAsync(this.Construct(query, parameters));
    public virtual async Task<bool?> LoadBoolAsync(string command)
      => await this.Loader.LoadSingleAsync<bool?>(command);

    ///
    /// Load boolean
    ///

    public virtual bool LoadBoolean(string query, params object[] parameters) { return this.LoadBoolean(this.Construct(query, parameters)); }
    public virtual bool LoadBoolean(string command)
    {
      string result = this.Loader.LoadSingle<string>(command);
      return result.Equals("1") || result.ToLower().Equals("true");
    }
    public virtual Task<bool> LoadBooleanAsync(string query, params object[] parameters) => this.LoadBooleanAsync(this.Construct(query, parameters));
    public virtual async Task<bool> LoadBooleanAsync(string command)
    {
      string result = await this.Loader.LoadSingleAsync<string>(command);
      return result.Equals("1") || result.ToLower().Equals("true");
    }

    ///
    /// Load guid
    ///

    public virtual Guid? LoadGuid(string query, params object[] parameters) { return this.LoadGuid(this.Construct(query, parameters)); }
    public virtual Guid? LoadGuid(string command)
      => this.Loader.LoadSingle<Guid?>(command);
    public virtual async Task<Guid?> LoadGuidAsync(string query, params object[] parameters) => await LoadGuidAsync(this.Construct(query, parameters));
    public virtual async Task<Guid?> LoadGuidAsync(string command)
      => await this.Loader.LoadSingleAsync<Guid?>(command);

    ///
    /// Load Datetime
    ///

    public virtual DateTime? LoadDateTime(string query, params object[] parameters) { return this.LoadDateTime(this.Construct(query, parameters)); }
    public virtual DateTime? LoadDateTime(string command)
      => this.Loader.LoadSingle<DateTime?>(command);
    public virtual async Task<DateTime?> LoadDateTimeAsync(string query, params object[] parameters) => await LoadDateTimeAsync(this.Construct(query, parameters));
    public virtual async Task<DateTime?> LoadDateTimeAsync(string command)
      => await this.Loader.LoadSingleAsync<DateTime?>(command);

    ///
    /// Load string
    ///

    public virtual string LoadString(string query, params object[] parameters) { return this.LoadString(this.Construct(query, parameters)); }
    public virtual string LoadString(string command)
      => this.Loader.LoadSingle<string>(command);
    public virtual async Task<string> LoadStringAsync(string query, params object[] parameters) => await LoadStringAsync(this.Construct(query, parameters));
    public virtual async Task<string> LoadStringAsync(string command)
      => await this.Loader.LoadSingleAsync<string>(command);

    ///
    /// Load container
    ///

    public virtual DirectContainer LoadContainer(string query, params object[] parameters) { return this.LoadContainer(this.Construct(query, parameters)); }
    public virtual DirectContainer LoadContainer(string command) => this.Load(command).Container;
    public virtual async Task<DirectContainer> LoadContainerAsync(string query, params object[] parameters) => (await this.LoadAsync(this.Construct(query, parameters))).Container;
    public virtual async Task<DirectContainer> LoadContainerAsync(string command) => (await this.LoadAsync(command)).Container;

  }
}

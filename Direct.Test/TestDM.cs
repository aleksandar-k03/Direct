using Direct.Models;
using System;

namespace Direct.ccmonkeys.Models
{
  public partial class TestDM : DirectModel
  {

    public TestDM(DirectDatabaseBase db) : base("tm_test", "testid", db) { }

    [DColumn(Name = "testid", IsPrimary = true)]
    public string testid { get; set; } = default;

    [DColumn(Name = "actionid")]
    public int actionid { get; set; } = default;

    [DColumn(Name = "guid")]
    public string guid { get; set; } = default;

    [DColumn(Name = "created", NotUpdatable = true, HasDefaultValue = true)]
    public DateTime created { get; set; } = default;

  }
}

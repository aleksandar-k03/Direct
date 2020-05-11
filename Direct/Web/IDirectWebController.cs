using Direct.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Web
{
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IDirectWebController<T> where T : DirectModel
  {

    // GET METHODS

    Task<IActionResult> GetSingle(string id);
    Task<IActionResult> Select(string value, int limit);


    Task<IActionResult> Update(string id);

    Task<IActionResult> Insert();


  }
}

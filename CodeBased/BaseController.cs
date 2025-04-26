using Microsoft.AspNetCore.Mvc;

namespace Kpakam.CodeBased
{
    public class BaseController : Controller
    {
        [NonAction]
        public void CheckKey(string Proof)
        {
            var AppValues = new ConfigurationBuilder().AddJsonFile(Constant.appSettings).Build();
            string myPass = AppValues.GetValue<string>(Constant.myPassKey); 
            if ((Proof ?? "") != (myPass ?? ""))
                throw new Exception("Unauthorized Request");
        } 
    }
}

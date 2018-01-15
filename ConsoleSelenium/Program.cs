using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using EF1;
using OpenQA.Selenium.PhantomJS;

namespace ConsoleSelenium
{
    class Program
    {
        static void Main(string[] args)
        {
            //IWebDriver driver = new FirefoxDriver();
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            IWebDriver driver = new PhantomJSDriver(driverService);

            Console.WriteLine("Datos procesados");
            Console.WriteLine("-----------------");
            using (baseprod2Entities context = new baseprod2Entities())
            {
                List <SP_S_LeadsNoProcesados_Scotiabank_Result> procesar = context.SP_S_LeadsNoProcesados_Scotiabank().ToList();
                foreach (var lead in procesar)
                {
                    driver.Navigate().GoToUrl("https://sac.scotiabank.indexa.cl/SAC/simular/Solicitud.asp?Tipo_Cuota=CI&tipocre=121&fpago=Cuenta+Corriente+&monto=" + 
                        lead.monto_credito +"&valor=" + lead.monto_credito + "&ncuotas=" + lead.nro_cuotas + "&primerpago=16%2F02%2F2018&mnp1=0&mnp2=0&SDesgravamen=&SCesantia=&Total_Financiar=3158266&Valor_Cuota=" + lead.valor_cuota + "&Impuesto_Pagare=25266&Monto_SeguroDesgravamen=0&Monto_SeguroCesantia=0&Total_Recargos=133000&canal_origen=");
                    IWebElement qname = driver.FindElement(By.Name("nombre"));
                    qname.SendKeys("Chile");

                    IWebElement qpaterno = driver.FindElement(By.Name("paterno"));
                    qpaterno.SendKeys("autos");

                    IWebElement qmaterno = driver.FindElement(By.Name("materno"));
                    qmaterno.SendKeys("SpA");

                    IWebElement qrut = driver.FindElement(By.Name("rut"));
                    qrut.SendKeys(lead.rut.Replace("-", ""));

                    IWebElement qtfijo = driver.FindElement(By.Name("tfijo"));
                    qtfijo.SendKeys(lead.telefono);

                    IWebElement qtcelular = driver.FindElement(By.Name("tcelular"));
                    qtcelular.SendKeys(lead.telefono);

                    IWebElement qemail = driver.FindElement(By.Name("email"));
                    qemail.SendKeys(lead.email);

                    IWebElement qrenta = driver.FindElement(By.Name("renta"));
                    qrenta.SendKeys("0");

                    IWebElement qfnacimiento = driver.FindElement(By.Name("fnacimiento"));
                    qfnacimiento.SendKeys("01/01/1998");

                    IWebElement qantiguedad = driver.FindElement(By.Name("antiguedad"));
                    qantiguedad.SendKeys("2");

                    IWebElement qempleador = driver.FindElement(By.Name("empleador"));
                    qempleador.SendKeys("Sin especificar");

                    IList<IWebElement> qtipocontratos =  driver.FindElements(By.Name("tipocontrato"));
                    qtipocontratos.ElementAt(0).Click();

                    
                    driver.FindElement(By.ClassName("btn-simula-form")).Click();
                    driver.Manage().Timeouts().ImplicitWait = new TimeSpan(10);

                    try
                    {
                        if (driver.FindElement(By.TagName("p")).Text.Contains("Nos comunicaremos al mail informado"))
                        {
                            //cambio el estado del lead
                            SP_U_MarcaLeads_Scotiabank_Result resultado = context.SP_U_MarcaLeads_Scotiabank(lead.rut, lead.telefono, lead.email, lead.monto_credito, lead.nro_cuotas, lead.valor_cuota).FirstOrDefault();
                            if (resultado != null && resultado.actualizado == 1)
                            {
                                Console.WriteLine("Rut:" + lead.rut + ", Teléfono:" + lead.telefono + ", email:" + lead.email + ", monto crédito:" + lead.monto_credito +
                                    ", cuotas:" + lead.nro_cuotas + ", valor cuota:" + lead.valor_cuota);
                            }
                        }
                    }
                    catch (NoSuchElementException e)
                    {
                        Console.WriteLine("Error Message = {0}", e.Message);
                    }
                    

                }
            }
            driver.Quit();
            Console.ReadKey();
        }
    }
}

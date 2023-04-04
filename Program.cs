using System.IO.Ports;
using System.Management;
using System.Text;

namespace PortConsol
{
    class Program
    {

        static SerialPort port;
        public static string ConfirmationReadiness = "ok"; // returns a device connected to the uart port reading data(a word confirming readiness to accept the next command)
        public static bool con = false;                    // loop condition


        static async Task Main(string[] args)
        {


            setup(); //sets the serial port

            // metoda asynhroniczna

            Task task1 = Task.Run(() => ProgramStop());
            Task task2 = Task.Run(() => SendData(ConfirmationReadiness));
            Task task3 = Task.Run(() => OpenAgain());
            await Task.WhenAll(task1, task2, task3);

        }




        static void setup()
        {

            int s = 0;

            while (true)
            {

                var instances = new ManagementClass("Win32_SerialPort").GetInstances();
                foreach (ManagementObject port in instances)
                {
                    Console.WriteLine("{0}: {1}", port["deviceid"], port["name"]);
                    s++;
                }
                if (s > 0)
                {
                    s = 0;
                    break;
                }
            }
            s = 0;
            Console.WriteLine("Enter the port number");
            int _portNum = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Enter the BaudRate");
            int _baudRate = Convert.ToInt32(Console.ReadLine());

            setPort(_portNum, _baudRate);



        }

        static void setPort(int portNum, int baudRate)
        {
            var ComPortName = "COM" + portNum;

            port = new SerialPort(ComPortName, baudRate, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
                Console.WriteLine("Connected",port);
                System.Threading.Thread.Sleep(2300);
                Console.Clear();

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Error: Port {0} is in use", ComPortName);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Uart exception: " + ex);
            }


        }

        static async Task SendData(string str) //accepts the word returned by the device (a word confirming readiness to accept the next command)
        {
            while (true)
            {
                string filePath;
                Console.WriteLine("Enter the path to the file:");
                filePath = Console.ReadLine();
                Console.Clear();
                try
                {
                    // Open the file for reading
                    using (StreamReader reader = new StreamReader(filePath))
                    {

                        string content;

                        while ((content = reader.ReadLine()) != null)
                        {

                            if (con == true) // interrupt function
                            {
                                break;

                            }

                            Console.WriteLine(content);
                            byte[] buffer = Encoding.ASCII.GetBytes(content);
                            port.BaseStream.WriteAsync(buffer, 0, buffer.Length);



                            string response = "";
                            while (!response.Contains(str))
                            {

                                if (con == true) // check if it can be interrupted
                                {
                                    break;

                                }
                                response = port.ReadLine();

                                Console.WriteLine(response);
                            }



                        }


                    }

                }

                catch (Exception ex)
                {
                    Console.WriteLine("Wystąpił błąd: " + ex.Message);
                }
                con = false;

            }
        }




        static async Task OpenAgain() // f button activates task SendData("");
        {

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.F)
                {
                    con = true; // condition to abort and load a new file
                }

            }



        }

        static async Task ProgramStop()
        {

            while (true)
            {

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);

                }

            }


        }
    }



}












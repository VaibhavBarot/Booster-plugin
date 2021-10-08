using System;
using System.Collections.Generic;
using System.IO;


namespace ArchiSteamFarm
{
    class ReadGames
    {
      
    
        
    

        public  static List<uint> getCraftingids(string fileName)
        {
			string textFile = "Gameids\\" + fileName + ".txt";
            List<uint> id = new List<uint>();

            if (File.Exists(textFile))
            {
               
                   List<string> stringList = new List<string>(File.ReadAllLines(textFile));
                    foreach (string i in stringList)
                    {
                        id.Add(Convert.ToUInt32(i));


                    }
                    return id;
                

            }
            else
            {
                return null;
            }
               
                
            
                
        }

		public static bool AddIds(string[] files, string[] ids)
			{
			
			foreach(string file in files)
				{
					string textFile = "Gameids\\" + file + ".txt";
				if (!File.Exists(textFile))
					{
					Console.WriteLine("File not found");
					}
				
				List<string> list = new List<string>(File.ReadAllLines(textFile));
					foreach (string id in ids)
					{
					if (!list.Contains(id))
					{
						list.Add(id);
					}
					
					}
				File.Delete(textFile);
				  File.WriteAllLines(textFile,list);
				 
				
				
				}
			return true;
			}
	
		public static bool RemoveIds(string[] files, string[] ids) {
			
			foreach (string file in files) {
				string textFile = "Gameids\\" + file + ".txt";
				if (!File.Exists(textFile)) {
					Console.WriteLine("File not found");
				}

				List<string> list = new List<string>(File.ReadAllLines(textFile));
				foreach (string id in ids) {
					if (list.Contains(id)) {
						list.Remove(id);
					}

				}
				File.Delete(textFile);
				File.WriteAllLines(textFile, list);



			}
			return true;
		}
	}
}

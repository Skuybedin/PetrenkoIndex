using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrenkoMain
{
    public class CalcIndex
    {
        private static readonly HashSet<char> rus = new("01234567890йцукенгшщзхъфывапролджэячсмитьбюёЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ");
        private static readonly HashSet<char> eng = new("0123456789qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM");
        private readonly HashSet<float> similarIndexes; //Будем использовать float, так как 1 символ будет занимать 4 байта, вместо 8 если использовать double
        private readonly Dictionary<float, List<string>> mainDict;

        public CalcIndex(Stream file)
        {
            mainDict = MakeAllIndexes(file);
            similarIndexes = MakeSimilarIndexes(mainDict);
        }

        private static float CalculateIndex(string line) //Метод для подсчета индекса
        {
            float index = 0.5F, result = 0;
            int count = 0;

            foreach (var letter in line)
            {
                if (eng.Contains(letter) || rus.Contains(letter))
                {
                    result += index;
                    index++;
                    count++;
                }
            }

            result *= count;

            return result;
        }

        private static Dictionary<float, List<string>> MakeAllIndexes(Stream file)
        {
            using var read = new StreamReader(file);
            Dictionary<float, List<string>> mainDict = new Dictionary<float, List<string>>();
            string sentence;

            while (((sentence = read.ReadLine()) != null) || ((sentence = read.ReadLine()) != string.Empty)) //Работаем до тех пор, пока не встретим null или пустоту в файле потока
            {
                string[] senArray = sentence.Split('|'); //Разделяем строку на предложение и комментарий
                float index = CalculateIndex(senArray[0]); //Считаем индекс предложения (без комментария)

                if (senArray.Length == 2) index += CalculateIndex(senArray[1]); //Если строка на английском языке, то считаем еще индекс комментария

                if (mainDict.TryGetValue(index, out List<string> value)) value.Add(sentence); //Пытаемся получить значение по ключу. Если значение есть, то добавляем в наш список еще строку
                else mainDict.Add(index, new List<string> { sentence }); //В противном случае добавляем в наш словарь новую пару ключ:значение
            }

            return mainDict;
        }

        private static HashSet<float> MakeSimilarIndexes(Dictionary<float, List<string>> mainDict) => mainDict.Select(x => x.Key).ToHashSet(); //Переведем наш словарь в хэш-множество
        public Dictionary<float, List<string>> GetSimilar() => similarIndexes.ToDictionary(x => x, x => mainDict[x].ToList()); //Переводим из множества в словарь
    }
}

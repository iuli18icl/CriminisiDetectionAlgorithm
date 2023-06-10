//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CriminisiDetectionAlgorithm
//{
//    public static FuzzyList ComputeFuzyList()
//    {
//        FuzzyList fuzzyList = new FuzzyList()
//        {
//            CreateFuzzyMember = new List<double>()
//        };

//        double endi = Math.Pow(fuzzyList.patchsize, 2);

//        for(int i = 0; i<= endi; i++)
//        {
//            if (i <= fuzzyList.a)
//            {
//                fuzzyList.CreateFuzzyMember.Add(0);
//            } 
            
//            else if (i > fuzzyList.b)
//            {
//                fuzzyList.CreateFuzzyMember.Add(1);
//            } 
            
//            else
//            {
//                double member = (i - fuzzyList.a) * 1.0 / (fuzzyList.b - fuzzyList.a);
//                fuzzyList.CreateFuzzyMember.Add(member);
//            }

//        }
//        return fuzzyList;

//    }
//}

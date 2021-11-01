using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Genetic_Algorithm
    {
        public int Scale;//问题规模
        public List<List<int>> population;//种群，4*Scale数量的棋盘
        public int queenPairs;//皇后总对数
        public int populationScale;//种群规模
        public List<int> indiFitness;//个体适应性
        List<double> indiSelPosi;//个体被选中作为双亲的概率
        List<double> accuPosi;//累计概率
        double pCross;
        double pMutate;
        public int generation;
        Random rnd;

        public Genetic_Algorithm(int Scale_,double pCross, double pMutate)
        {
            this.Scale = Scale_;
            this.pCross = pCross;
            this.pMutate = pMutate;
            queenPairs = 0;
            rnd = new Random();
            for (int i = Scale - 1; i > 0; i--) queenPairs+=i;
            populationScale = 4 * Scale_;
            this.setPopulation();//设置种群
            this.calFitAndPosi();//计算每个棋盘的适应度以及被选中的概率
            generation = 0;
        }
        public void setPopulation()
        {
            population = new List<List<int>>();
            population.Clear();
            for (int i = 0; i < populationScale; i++)   population.Add(setChessBoard());
        }
        public List<int> setChessBoard()
        {
            List<int> chessBoard = new List<int>();
            chessBoard.Clear();
            for (int j = 0; j < Scale; j++) chessBoard.Add(j);
            Random rnd = new Random();
            rnd = new Random();
            for (int row1 = 0; row1 < Scale; row1++)
            {
                int row2 = rnd.Next(0, Scale - 1);
                int tmp1 = chessBoard[row1];
                int tmp2 = chessBoard[row2];
                chessBoard[row1] = tmp2;
                chessBoard[row2] = tmp1;
            }
            return chessBoard;
        }
        
        public int calNonConflict(List<int> chessBoard)
        {
            int conflicts = 0;
            for (int i = 0; i < Scale; i++)
            {
                for (int j = i + 1; j < Scale; j++)
                {
                    if (Math.Abs(i - j) == Math.Abs(chessBoard[i] - chessBoard[j])) conflicts++; //斜线冲突
                    if (chessBoard[i] == chessBoard[j]) conflicts++; //横线冲突
                }
            }
            return queenPairs - conflicts;
        }

        public void calFitAndPosi()
        {
            indiFitness = new List<int>();
            indiFitness.Clear();
            indiSelPosi = new List<double>();
            indiSelPosi.Clear();
            accuPosi = new List<double>();
            accuPosi.Clear();

            int sumOfPosi = 0;
            foreach(List<int> chessBoard in population)
            {
                int nonCon = calNonConflict(chessBoard);
                indiFitness.Add(nonCon);
                sumOfPosi += nonCon;
            }
            foreach(int fit in indiFitness)  indiSelPosi.Add(1.0 * fit / sumOfPosi);//轮盘赌算法选择概率计算
            for(int i = 0; i < populationScale; i++)//轮盘赌算法累积概率计算
            {
                double sum = 0;
                for (int j = 0; j <= i; j++) sum += indiSelPosi[j];
                accuPosi.Add(sum);
            }
        }

        public List<List<int>> generateNewPopulation()
        {
            List<List<int>> newPopulation = new List<List<int>>();
            newPopulation.Clear();
            while (newPopulation.Count < populationScale)
            {
                List<int> son = generateASon();
                newPopulation.Add(son);
            }
            return newPopulation;
        }

        public List<int> generateASon_()
        {
            //Random rnd = new Random();
            while (true)//如果一遍没有产生优于双亲的孩子，则继续繁殖过程
            {
                List<List<int>> parents = new List<List<int>>();
                parents.Clear();
                while (parents.Count != 2) 
                {
                    parents.Clear();
                    parents = chooseParents(ref parents);
                }

                List<int> son = new List<int>();
                son.Clear();

                double posi = rnd.NextDouble();

                if (posi < pCross) son = cross(parents);
                else son = parents[0];

                double posi_1 = rnd.NextDouble();
                if (posi_1 < pMutate) mutate(son);

                int son_non_conflict = calNonConflict(son);
                if (son_non_conflict > calNonConflict(parents[0]) && son_non_conflict > calNonConflict(parents[1])) return son;
                else if (pMutate <= 0.98) pMutate += 0.02;
            }
        }

        public List<int> generateASon()
        {
            //Random rnd = new Random();
            
            List<List<int>> parents = new List<List<int>>();
            parents.Clear();
            while (parents.Count != 2)
            {
                parents.Clear();
                parents = chooseParents(ref parents);
            }

            List<int> son = new List<int>();
            son.Clear();

            double posi = rnd.NextDouble();

            if (posi < pCross) son = cross(parents);
            else son = parents[0];

            double posi_1 = rnd.NextDouble();
            if (posi_1 < pMutate) mutate(son);

            //int son_non_conflict = calNonConflict(son);
            //pMutate += 0.02;
            return son;
                //if (son_non_conflict > calNonConflict(parents[0]) && son_non_conflict > calNonConflict(parents[1])) return son;
               
        }

        //roulette wheel selection轮盘赌算法
        public List<List<int>> chooseParents(ref List<List<int>> parents_tmp)
        {
            Random rnd = new Random();
            parents_tmp.Clear();
            for (int i = 0; i < 2; i++)
            {
                double posi = rnd.NextDouble();
                for (int j = 0; j < populationScale; j++)
                {
                    if (posi >= accuPosi[j] && posi < accuPosi[j+1])
                    {
                        parents_tmp.Add(population[j]);
                        break;
                    }
                }
            }
            return parents_tmp;
        }

        public List<int> cross(List<List<int>> parents)
        {
            List<int> son = new List<int>();
            son.Clear();
            Random rnd = new Random();
            int breakpoint1 = rnd.Next(0, Scale - 1);
            int breakpoint2 = rnd.Next(0, Scale - 1);
            while (breakpoint1 >= breakpoint2)
            {
                breakpoint1 = rnd.Next(0, Scale - 1);
                breakpoint2 = rnd.Next(0, Scale - 1);
            }
            for(int i = 0; i < breakpoint1; i++)
            {
                son.Add(parents[0][i]);
            }
            for(int i = breakpoint1; i < breakpoint2; i++)
            {
                son.Add(parents[1][i]);
            }
            for (int i = breakpoint2; i < Scale; i++)
            {
                son.Add(parents[0][i]);
            }
            return son;
        }
        public void mutate(List<int> son)
        {
            Random rnd = new Random();
            int col = rnd.Next(0, Scale - 1);
            int row = rnd.Next(0, Scale - 1);
            son[col] = row;
        }

        public void print(List<int> chessBoard)
        {
            for (int i = 0; i < Scale; i++) Console.Write("---");
            Console.WriteLine();

            for (int i = 0; i < Scale; i++)
            {
                int pos = chessBoard.FindIndex(item => item.Equals(i));
                for (int j = 0; j < pos; j++) Console.Write(" = ");
                Console.Write(" Q ");
                for (int k = pos; k < Scale - 1; k++) Console.Write(" = ");
                Console.WriteLine();
            }

            for (int i = 0; i < Scale; i++) Console.Write("---");
            Console.WriteLine();
        }

        public void solve()
        {
            List<int> sol = new List<int>();
            bool quit = false;
            while (true)
            {
                List<List<int>> newPopulation = generateNewPopulation();
                population.Clear();
                foreach (List<int> i in newPopulation) population.Add(i);
                
                calFitAndPosi();
                for (int i = 0; i < populationScale; i++)
                {
                    if (indiFitness[i] == queenPairs)
                    {
                        sol = population[i];
                        quit=true;
                    }
                }
                if (quit) break;
            }
            print(sol);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Genetic_Algorithm sol = new Genetic_Algorithm(18,0.9,0.2);
            List<int> sol_ = new List<int>();
            bool quit = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!quit)
            {
                List<List<int>> newPopulation = sol.generateNewPopulation();
                sol.population.Clear();
                foreach (List<int> i in newPopulation) sol.population.Add(i);
                sol.generation++;
                sol.calFitAndPosi();
                for (int i = 0; i < sol.populationScale; i++)
                {
                    if (sol.indiFitness[i] == sol.queenPairs)
                    {
                        sol_ = sol.population[i];
                        quit = true;
                    }
                }
            }
            sw.Stop();
            sol.print(sol_);
            Console.WriteLine("TIME:" + sw.Elapsed.TotalSeconds.ToString());
            Console.WriteLine("GEN:" + sol.generation.ToString());

            Console.WriteLine("Hello World!");
        }
    }
}

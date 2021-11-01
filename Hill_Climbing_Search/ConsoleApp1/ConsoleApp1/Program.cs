using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Random_Reset_Climbing
    {
        int Scale;
        List<int> chessBoard;
        public int maxSteps;
        public int step;
        public int columnPoisition=0;

        public Random_Reset_Climbing(int Scale)
        {
            this.Scale = Scale;
            maxSteps = Scale * Scale * 2;
            step = 0;
            this.setChessBoard();
        }
        public void setChessBoard()
        {
            chessBoard = new List<int>();
            chessBoard.Clear(); 
            for (int i = 0; i < Scale; i++) chessBoard.Add(i);
            Random rnd = new Random();
            for(int row1 = 0; row1 < Scale; row1++)
            {
                int row2 = rnd.Next(0, Scale - 1);
                int tmp1 = chessBoard[row1];
                int tmp2 = chessBoard[row2];
                chessBoard[row1] = tmp2;
                chessBoard[row2] = tmp1;
            }
            step = 0;
        }

        /*
        public int getConflicts()
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
            return conflicts;
        }
        */
        ///*
        public int getConflicts()
        {
            int conflicts = 0;
            for (int i = 0; i <= Scale - 1; i++)
            {
                for (int j = Scale - 1; j >= 0; j--)
                {
                    if (i == j) continue;
                    if (Math.Abs(i - j) == Math.Abs(chessBoard[i] - chessBoard[j])) conflicts++; //斜线冲突
                    if (chessBoard[i] == chessBoard[j]) conflicts++; //横线冲突
                }
            }//两重循环，全局计算 冲突对数被加倍
            return conflicts / 2;//所以返回值的时候需要除以二
        }
        //*/
        public int searchColumnMinConflict(int column)//查找每列的最小冲突点，如果有多个，则随机选择后返回
        {
            int best;
            int originalPosition = chessBoard[column];
            List<int> conflictCounter = new List<int>();
            for(int tmpRow = 0; tmpRow < Scale - 1; tmpRow++)
            {
                chessBoard[column] = tmpRow;
                conflictCounter.Add(getConflicts());
            }//计算一列中每个点的冲突值
            chessBoard[column] = originalPosition;//恢复原先的位置

            int minConflict= conflictCounter[0];
            List<int> bestPosition = new List<int>();
            bestPosition.Add(0);

            for (int tmpRow = 0; tmpRow < Scale - 1; tmpRow++)
            {
                if (conflictCounter[tmpRow] < minConflict)
                {
                    bestPosition.Clear();
                    bestPosition.Add(tmpRow);
                    minConflict = conflictCounter[tmpRow];
                }//如果找到了一个比目前的冲突值更小的点，则清除原先的列表，重新将新找的节点加入并更新最小冲突
                else if (conflictCounter[tmpRow] == minConflict)
                {
                    bestPosition.Add(tmpRow);
                }//如果找到了一个跟目前冲突值相同的点，则将其加入最佳节点列表
            }
            if (bestPosition.Count == 1) best = bestPosition[0];
            else
            {
                Random rnd = new Random();
                best = bestPosition[rnd.Next(0, ((bestPosition.Count) - 1))];
            }
            return best;
        }
        public void optimizing(int column)
        {
            chessBoard[column] = searchColumnMinConflict(column);
            step++;
        }

        public void print()
        {
            /*
             * for (int i = 0; i < N; i++) {
		            int num = chessboard[i];
		            for (int j = 0; j < num; j++) {
			            cout << " = ";
		            }
		            cout << " Q ";
		            for (int k = num + 1; k < N; k++) {
			            cout << " = ";
		            }
		            cout << endl;
	           }
             */

            for (int i = 0; i < Scale; i++) Console.Write("---");
            Console.WriteLine();

            for (int i = 0; i < Scale; i++)
            {
                int pos = chessBoard.FindIndex(item => item.Equals(i));
                for (int j = 0; j < pos; j++) Console.Write(" = ");
                Console.Write(" Q ");
                for (int k = pos; k < Scale-1; k++) Console.Write(" = ");
                Console.WriteLine();
            }

            for (int i = 0; i < Scale; i++) Console.Write("---");
            Console.WriteLine();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! Please input the number of QUEENS to be solved:");
            int scale=int.Parse(Console.ReadLine());
            Random_Reset_Climbing sol = new Random_Reset_Climbing(scale);
            sol.setChessBoard();
            int restTime = 0;
            while (sol.getConflicts() > 0)
            {
                if (sol.step > sol.maxSteps)
                {
                    sol.setChessBoard();
                    restTime++;
                    Console.WriteLine("第{0}次随机重启", restTime);
                }
                sol.optimizing(sol.columnPoisition++);
                if (sol.columnPoisition >= scale ) sol.columnPoisition = sol.columnPoisition % scale;
            }
            Console.WriteLine("Success!");
            sol.print();
        }
    }
}

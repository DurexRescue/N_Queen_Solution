using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Random_Reset_Climbing
    {
        public int Scale;
        List<int> chessBoard;
        public int maxSteps;
        public int step;
        public int columnPoisition = 0;
        bool change = false;

        List<int> mainQueens;
        List<int> secondQueens;
        List<int> rowQueens;

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

            mainQueens = new List<int>();
            mainQueens.Clear();

            secondQueens = new List<int>();
            secondQueens.Clear();

            rowQueens = new List<int>();
            rowQueens.Clear();

            for (int i = 0; i < Scale; i++) chessBoard.Add(i);
            Random rnd = new Random();
            for (int row1 = 0; row1 < Scale; row1++)
            {
                int row2 = rnd.Next(0, Scale - 1);
                int tmp1 = chessBoard[row1];
                int tmp2 = chessBoard[row2];
                chessBoard[row1] = tmp2;
                chessBoard[row2] = tmp1;
            }
            step = 0;
            change = true;
            int origin = getConflicts();
        }
        
        public void calMainQueen()//主对角线（左上到右下的线）总共2n-1条，从右上角的对角线开始计数
        {
            int queens = 0;
            mainQueens.Clear();//非常重要！！每次计算前需要清空List表，否则将导致错误（Bug Fixes）
            for(int tmp = Scale - 1; tmp >= 0; tmp--)//该循环计算从0到n-1号主对角线的皇后数,
                                                     //以第0行为基准，tmp为列标，共n条对角线
            {
                queens = 0;
                if (chessBoard[tmp] == 0) queens++;//如果第tmp列的皇后在第0行，则加一
                for(int tmp_1 = tmp + 1; tmp_1 <= Scale - 1; tmp_1++)
                {
                    if (tmp_1 > Scale - 1) break;
                    if (tmp_1 - tmp == chessBoard[tmp_1]) queens++;
                }
                mainQueens.Add(queens);
            }
            for(int tmp = 1; tmp <= Scale - 1; tmp++)//该循环计算从n到2n-2号主对角线的皇后数，
                                                     //以第0列为基准，tmp为行标，共n-1条
            {
                queens = 0;
                if (chessBoard[0] == tmp) queens++;//如果第0列的皇后在第tmp行，则加一
                for (int tmp_1 = tmp + 1; tmp_1 <= Scale - 1; tmp_1++)
                {
                    if (tmp_1 - tmp == chessBoard[tmp_1-tmp]-tmp) queens++; //列数之差(tmp_1-tmp)为何为行数之差的值？因为两者相等
                                                                            //等于行数之差()
                }
                mainQueens.Add(queens);
            }
            
        }

        
        public void calSecondQueen()//副对角线（右上到坐下的线）总共2n-1条，从左上角格子的对角线开始计数
        {
            int queens = 0;
            secondQueens.Clear();
            for(int tmp = 0; tmp <= Scale - 1; tmp++)//该循环计算从0到n-1号对角线的皇后数
                                                     //以第0行为基准，tmp为列标，共n条对角线
            {
                queens = 0;
                if (chessBoard[tmp] == 0) queens++;//如果第tmp列的皇后在第0行，则加一

                for (int tmp_1 = tmp - 1; tmp_1 >= 0; tmp_1--)
                {
                    if (tmp_1 < 0) break;
                    if (Math.Abs(tmp_1 - tmp) == chessBoard[tmp_1]) queens++;
                }
                secondQueens.Add(queens);

            }

            for (int tmp = 1; tmp <= Scale - 1; tmp++)//该循环计算从n到2n-2号主对角线的皇后数，
                                                      //以第0列为基准，tmp为行标，共n-1条
            {
                queens = 0;
                if (chessBoard[Scale-1] == tmp) queens++;//如果第Scale-1列的皇后在第tmp行，则加一
                for (int tmp_1 = tmp + 1; tmp_1 <= Scale - 1; tmp_1++)
                {
                    if (tmp_1 - tmp == chessBoard[Scale-1-(tmp_1 - tmp)] - tmp) queens++; //列数之差(tmp_1-tmp)为何为行数之差的值？因为两者相等
                                                                                //等于行数之差()
                }
                secondQueens.Add(queens);
            }

        }

        public void calRowQueen()
        {
            int queens = 0;
            rowQueens.Clear();
            for(int row = 0; row <= Scale - 1; row++)
            {
                queens = 0;
                foreach(int i in chessBoard)
                {
                    if (i == row) queens++;
                }
                rowQueens.Add(queens);
            }
        }

        public int getIndividualConflicts(int i)//参数列表的i代表第i列，i取值从0到Scale-1
        {
            int j = chessBoard[i];
            int mainNo = Scale - 1 - i + j;
            int secondNo = i + j;

            int mainConflict = 0;
            int secondConflict = 0;
            int rowConflicts = 0;

            if (mainQueens[mainNo] != 1) mainConflict = mainQueens[mainNo] - 1;
            if (secondQueens[secondNo] != 1) secondConflict = secondQueens[secondNo] - 1;
            if (rowQueens[j] != 1) rowConflicts = rowQueens[j] - 1;
            return mainConflict+ secondConflict+ rowConflicts;
        }

        public int getConflicts()
        {
            if (change) {
                calMainQueen();
                calRowQueen();
                calSecondQueen();
                change = false;
            }
            int conflicts = 0;
            for (int i = 0; i <= Scale - 1; i++) conflicts += getIndividualConflicts(i);
            return conflicts / 2;
        }
        
        public int searchColumnMinConflict(int column)//查找每列的最小冲突点，如果有多个，则随机选择后返回
        {
            int best;
            int originalPosition = chessBoard[column];
            List<int> conflictCounter = new List<int>();
            for (int tmpRow = 0; tmpRow <= Scale - 1; tmpRow++)
            {
                chessBoard[column] = tmpRow;
                change = true;
                conflictCounter.Add(getConflicts());
            }//计算一列中每个点的冲突值
            chessBoard[column] = originalPosition;//恢复原先的位置
            change = true;
            int minConflict = conflictCounter[0];
            List<int> bestPosition = new List<int>();
            bestPosition.Add(0);

            for (int tmpRow = 0; tmpRow <= Scale - 1; tmpRow++)
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
            change = true;
            step++;
        }

        public void print()
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
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! Please input the number of QUEENS to be solved:");
            int scale = int.Parse(Console.ReadLine());
            Random_Reset_Climbing sol = new Random_Reset_Climbing(scale);
            sol.setChessBoard();
            Console.WriteLine("Original ChessBoard:!");
            sol.print();
            int restTime = 0;
            while (sol.getConflicts() > 0)
            {
                
                if (sol.step > sol.maxSteps)
                {
                    sol.setChessBoard();
                    restTime++;
                    Console.WriteLine("第{0}次随机重启:", restTime);
                    sol.print();
                }
                
                sol.optimizing(sol.columnPoisition++);
                if (sol.columnPoisition >= scale) sol.columnPoisition = sol.columnPoisition % scale;
            }
            Console.WriteLine("Success! Result ChessBoard:");
            sol.print();
            Console.WriteLine(sol.step);
        }
    }
}

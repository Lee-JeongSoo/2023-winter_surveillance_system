﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace surveillance_system
{
   public partial class Program
    {
        public class Road
        {
            public double[] laneVector;

            // 가로 도로 좌표
            // 첫번째 인덱스 = 도로 번호,  두번째 인덱스 = y 값
            public double[,] lane_h; // 가로 - 중앙선 y 값 
            public double[,] lane_h_upper; // 가로 - 중앙선 위 라인 y값 
            public  double[,] lane_h_lower; // 가로 - 중앙선 아래 라인 y값 

            // 세로 도로 좌표
            // 첫번째 인덱스 = 도로 번호,  두번째 인덱스 = x 값
            public double[,] lane_v; // 세로 - 중앙선 x값
            public double[,] lane_v_left; // 세로- 중앙선 왼쪽 라인 x값
            public double[,] lane_v_right; // 세로 중앙선 오른쪽 라인 x값

            public double[,] DST; // 도로 교차점
            public double[,] intersectionArea; // 도로 교차구간
            public int mapSize;
            public int lane_num;
            public int interval;
            public int width;

            public void roadBuilder(int wd, int intvl, int n_interval, int n_cctv, int n_ped, int n_car)
            {
                this.lane_num = n_interval + 1;
                DST = new double[lane_num * lane_num, 2];
                intersectionArea = new double[lane_num * lane_num, 4];
                this.interval = intvl;
                this.width = wd;

                this.mapSize = n_interval * intvl + wd * lane_num;
                // 교차점, 교차구간 설정
                int idx = 0;
                for (int i = 0; i < lane_num; i++)
                {
                    for (int j = 0; j < lane_num; j++)
                    {
                        DST[idx, 0] = (intvl + wd) * i + (wd / 2);
                        DST[idx, 1] = (intvl + wd) * j + (wd / 2);

                        intersectionArea[idx, 0] = DST[idx, 0] - (wd / 2); // x_min
                        intersectionArea[idx, 1] = DST[idx, 0] + (wd / 2); // x_max
                        intersectionArea[idx, 2] = DST[idx, 1] - (wd / 2); // y_min
                        intersectionArea[idx, 3] = DST[idx, 1] + (wd / 2); // y_max
                        idx++;
                    }
                }

                // 도로 벡터 초기화
                double incr = 100;
                int laneVectorSize = (int)((intvl + wd) * (n_interval) / incr);
                //Console.WriteLine("laneSize = {0}", laneSize);
                laneVector = new double[laneVectorSize];

                for (int i = 0; i < laneVectorSize; i++)
                {
                    laneVector[i] = i * incr;
                }

                // 가로 도로 좌표 설정
                lane_h = new double[lane_num, 1];
                lane_h_upper = new double[lane_num, 1];
                lane_h_lower = new double[lane_num, 1];

                for (int i = 0; i < lane_num; i++)
                {
                    lane_h[i, 0] = i * (intvl + wd) + (wd / 2);
                    lane_h_upper[i, 0] = lane_h[i, 0] + wd / 2;
                    lane_h_lower[i, 0] = lane_h[i, 0] - wd / 2;
                }

                // 세로 도로 좌표 설정
                lane_v = new double[lane_num, 1];
                lane_v_left = new double[lane_num, 1];
                lane_v_right = new double[lane_num, 1];
                for (int i = 0; i < lane_num; i++)
                {
                    lane_v[i, 0] = i * (intvl + wd) + (wd / 2);
                    lane_v_left[i, 0] = lane_h[i, 0] - wd / 2;
                    lane_v_right[i, 0] = lane_h[i, 0] + wd / 2;
                }

                setCCTV(n_cctv, wd, lane_num);
                setPed(n_ped);
                setCar(n_car);
            }

            public void setCCTV(int n_cctv, int wd, int n_interval)
            {
                int[,] cctvPos = new int[52,52];


                double range = mapSize - width;
                int rootN = (int)Math.Sqrt((double)n_cctv);

                // x좌표가 int 형식이라 캐스팅해서 완벽한 그리드는 아닐 수 있음
                int intvl = (int)range / (rootN-1); 
                Console.WriteLine("mapsize range rootN intvl {0} {1} {2} {3} ", mapSize, range, rootN, intvl);
                double startX = DST[0, 0];
                double startY = DST[0, 1];

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position");
                int cctvIdx = 0;
                for(int i = 0; i < rootN; i ++)
                {
                    startX = DST[0, 0];
                    for (int j = 0; j < rootN; j++)
                    {
                        cctvs[cctvIdx].X = (int)startX;
                        cctvs[cctvIdx].Y = (int)startY;
                        Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", i * rootN + j, cctvs[cctvIdx].X , cctvs[cctvIdx].Y);
                        // Console.WriteLine("pos arr {0} {1} ", cctvs[i].Y / 10000, cctvs[i].X / 10000);
                        Console.WriteLine();
                        startX += intvl;

                        //debug
			            cctvPos[(cctvs[cctvIdx].Y)/10000, (cctvs[cctvIdx].X)/10000] += 1;
                        
                        cctvIdx++;

                    }

                    startY += intvl;
                			// 여기는 cctv 값 넣는 for문 안쪽
                }
                // for문 끝나고

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print CCTV Position");
                for (int i = 0 ; i < 52; i++) {
                      Console.Write("{0, 2}",i);
                }
                Console.WriteLine();
                for(int i = 0 ; i < 52; i++) {
                  Console.Write("{0, 2}",i);

                  for(int j = 0 ; j < 52; j++) {
                    if(cctvPos[i,j] <= 0) 
                      // Console.Write("{0, 2}", " ", cctvPos[i,j]);
                      Console.Write("{0, 2}", " ");
                    else
                      // Console.Write("{0, 2}", "*", cctvPos[i,j]);
                      Console.Write("{0, 2}", cctvPos[i, j]);

                }
                  Console.WriteLine();
                }
            }

            // 보행자 위치 처음 설정
            public void setPed(int n_ped)
            {
                int[,] pedPos = new int[52, 52];
                for (int i = 0; i < n_ped; i++)
                {
                    Random rand = new Random();
                    // int intersectidx = rand.Next(9);
                    int intersectidx = rand.Next(lane_num * lane_num);

                    // Console.WriteLine(intersectidx);
                    double[,] newPos = getPointOfAdjacentRoad(intersectidx);
                    peds[i].X = Math.Round(newPos[0, 0]);
                    peds[i].Y = Math.Round(newPos[0, 1]);

                    /*Random rand = new Random();
                    double opt = rand.NextDouble();

                    if (opt > 0.5) {
                        peds[i].X = Math.Round(laneVector.Max() * opt);
                        peds[i].Y = lane_h[rand.Next(0, lane_h.GetLength(0)), 0];
                    }
                    else
                    {
                        peds[i].X =lane_v[rand.Next(0, lane_v.GetLength(0)), 0];
                        peds[i].Y = Math.Round(laneVector.Max() * opt);
                    }*/
                    pedPos[Convert.ToInt32((peds[i].Y) / 10000), Convert.ToInt32((peds[i].X / 10000))] += 1;
                }
                // for문 끝나고

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print Pedestrian Position");
                for (int i = 0; i < 52; i++)
                {
                    Console.Write("{0, 2}", i);
                }
                Console.WriteLine();
                for (int i = 0; i < 52; i++)
                {
                    Console.Write("{0, 2}", i);

                    for (int j = 0; j < 52; j++)
                    {
                        if (pedPos[i, j] <= 0)
                            // Console.Write("{0, 2}", " ", pedPos[i, j]);
                            Console.Write("{0, 2}", " ");
                        else
                            // Console.Write("{0, 2}", "&", pedPos[i, j]);
                            Console.Write("{0, 2}", pedPos[i, j]);

                    }
                    Console.WriteLine();
                }
            }

            // set Car object
            public void setCar(int n_car)
            {
                int[,] carPos = new int[52, 52];
                for (int i = 0; i < n_car; i++)
                {
                    Random rand = new Random();
                    int intersectidx = rand.Next(lane_num * lane_num);
                    cars[i].X = DST[intersectidx, 0];
                    cars[i].Y = DST[intersectidx, 1];

                    int carintersectidx = rand.Next(4); // 0, 1, 2, 3
                    if (carintersectidx == 0)
                    {// down left
                        cars[i].X -= width / 4;
                        cars[i].Y += width / 4;
                    }
                    else if (carintersectidx == 1)
                    {// up left
                        cars[i].X += width / 4;
                        cars[i].Y += width / 4;
                    }
                    else if (carintersectidx == 2)
                    {// up right
                        cars[i].X += width / 4;
                        cars[i].Y -= width / 4;
                    }
                    else if (carintersectidx == 3)
                    {// down right
                        cars[i].X -= width / 4;
                        cars[i].Y -= width / 4;
                    }
                    carPos[Convert.ToInt32((cars[i].Y) / 10000), Convert.ToInt32((cars[i].X / 10000))] += 1;
                }
                // for문 끝나고

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print Car Position");
                for (int i = 0; i < 52; i++)
                {
                    Console.Write("{0, 2}", i);
                }
                Console.WriteLine();
                for (int i = 0; i < 52; i++)
                {
                    Console.Write("{0, 2}", i);

                    for (int j = 0; j < 52; j++)
                    {
                        if (carPos[i, j] <= 0)
                            // Console.Write("{0, 2}", " ", carPos[i, j]);
                            Console.Write("{0, 2}", " ");
                        else
                            // Console.Write("{0, 2}", "$", carPos[i, j]);
                            Console.Write("{0, 2}", carPos[i, j]);

                    }
                    Console.WriteLine();
                }
            }

            public double[,] getPointOfAdjacentRoad(int currAreaIdx)
            {
                if (currAreaIdx == -1)
                {
                    return new double[,] { { 0, 0 } };
                }

                int i, j;
                Random rand = new Random();

                do
                {
                    i = currAreaIdx / lane_num;
                    j = currAreaIdx % lane_num;

                    int opt = rand.Next(0, 4);
                    if (opt == 0) j += 1; // up
                    else if (opt == 1) j -= 1; // down
                    else if (opt == 2) i -= 1; // left
                    else if (opt == 3) i += 1; // right

                } while (i < 0 || i >= lane_num || j < 0 || j >= lane_num);

                int idx = lane_num * i + j;
                double[,] newPos = new double[1, 2];
                // newPos[0,0] = DST[idx, 0] + rand.Next(-width, width) * rand.NextDouble();
                // newPos[0,1] = DST[idx, 1] + rand.Next(-width, width) * rand.NextDouble();

                //20220512
                // newPos[0, 0] = rand.Next((int)intersectionArea[idx, 0], (int)intersectionArea[idx, 1]);
                // newPos[0, 1] = rand.Next((int)intersectionArea[idx, 2], (int)intersectionArea[idx, 3]);

                newPos[0, 0] = DST[idx, 0];
                newPos[0, 1] = DST[idx, 1];

                // Console.WriteLine("newpos {0} {1}", newPos[0, 0], newPos[0, 1]);
                return newPos;
            }

            public int getIdxOfIntersection(double x, double y)
            {
                for (int i = 0; i < intersectionArea.GetLength(0); i++)
                {
                    if (x >= intersectionArea[i, 0] && x <= intersectionArea[i, 1] && y >= intersectionArea[i, 2] && y <= intersectionArea[i, 3])
                    {
                        // Console.WriteLine("getIdxIntersection return {0}", i);
                        return i;
                    }
                }

                return -1;
            }

            public double[,] getPointOfAdjacentIntersection(int currAreaIdx, double x, double y)
            {
                if(currAreaIdx == -1){
                    return new double[,] { { 0, 0 } };
                }

                int i, j;
                double curX, curY;
                double midX = DST[currAreaIdx, 0];
                double midY = DST[currAreaIdx, 1];

                Random rand = new Random();

                // Console.WriteLine("getPointOfAdjacentIntersection x: {0}, y: {1}, midX: {2}, midY: {3}", x, y, midX, midY);
                do
                {
                    i = currAreaIdx / lane_num;
                    j = currAreaIdx % lane_num;
                    curX = x;
                    curY = y;

                    int opt = rand.Next(0, 2);

                    // 이 부분 수정 필요
                    if ( x < midX && y >= midY ){ // 0 down left
                        if (opt == 0)// down
                        {
                            curY -= width/2;
                            // curY -= (interval + width / 2);
                            // j -= 1;
                        }
                        else if (opt == 1) //left
                        { 
                            curX -= (interval + width/2);
                            i -= 1;
                        }
                    }
                    else if ( x >= midX && y >= midY ){ // 1 up left
                        if (opt == 0) // up
                        {   
                            curY += (interval + width/2);
                            j += 1; 
                        }
                        else if (opt == 1) // left
                        {
                            curX -= width/2;
                            // curX -= (interval + width / 2);
                            // i -= 1;
                        }
                    }
                    else if ( x >= midX && y < midY ){ // 2 up right
                        if (opt == 0) // up
                        {
                            curY += width/2;
                            // curY += (interval + width / 2);
                            // j += 1;
                        }
                        else if (opt == 1) // right
                        {
                            curX += (interval + width/2);
                            i += 1;
                        }                        
                    }
                    else if( x < midX && y < midY ){ // 3 down right
                        if (opt == 0) // down
                        {
                            curY -= (interval + width/2);
                            j -= 1;
                        }
                        else if (opt == 1) // right
                        {
                            curX += width/2;
                            // curX += (interval + width / 2);
                            // i += 1;
                        }
                    }

                } while (i< 0 || i >= lane_num || j < 0|| j >=  lane_num);

                int idx = lane_num * i + j;
                double[,] newPos = new double[1, 2];

                newPos[0, 0] = curX;
                newPos[0, 1] = curY;

                return newPos;
            }

            public void printRoadInfo()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "DST");
                for (int i = 0; i < DST.GetLength(0); i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0) Console.Write("DST[{0, 2}].X = {1, 6}\t", i, DST[i, j]);
                        if (j == 1) Console.Write("DST[{0, 2}].Y = {1, 6}\t", i, DST[i, j]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("\n========================================================================================\n");

                /* -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                   Console.WriteLine("\n========================lane Vector========================================");
                   for (int i = 0; i < laneVector.Length; i++)
                   {
                       Console.Write(laneVector[i] + "  ");
                       if (i % 99 == 0) Console.WriteLine();
                   }
                   Console.WriteLine("\n===========================================================================\n");
                ------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "lane horizontal");
                for (int i = 0; i < lane_h.GetLength(0); i++)
                {
                    Console.WriteLine("\n{0}번째 가로 도로 정보", i);
                    Console.WriteLine("y좌표 : 위 - 중앙 - 아래");
                    Console.WriteLine("       {0}   {1}   {2}", lane_h_upper[i,0],lane_h[i,0],lane_h_lower[i,0]);
                }
                Console.WriteLine("\n========================================================================================\n");

                

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "lane vertical");
                for (int i = 0; i < lane_h.GetLength(0); i++)
                {
                    Console.WriteLine("\n{0}번째 세로 도로 정보", i);
                    Console.WriteLine("x좌표 : 왼쪽 - 중앙 - 오른쪽");
                    Console.WriteLine("       {0}   {1}   {2}", lane_v_left[i, 0], lane_v[i, 0], lane_v_right[i, 0]);
                }
                Console.WriteLine("\n========================================================================================\n");
            }
        }
    }
}
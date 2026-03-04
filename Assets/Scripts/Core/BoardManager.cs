using System.Collections.Generic;
using UnityEngine;
using DragonChessLegends.Core;

namespace DragonChessLegends.Core
{
    /// <summary>
    /// 棋盘管理
    /// 9列 x 10行
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; private set; }
        
        // 棋盘尺寸
        public const int Width = 9;
        public const int Height = 10;
        
        // 楚河汉界 (第5行，0-indexed为4)
        public const int RiverRow = 4;
        
        // 九宫区域
        public const int RedPalaceMinX = 3;
        public const int RedPalaceMaxX = 5;
        public const int RedPalaceMinY = 0;
        public const int RedPalaceMaxY = 2;
        
        public const int BlackPalaceMinX = 3;
        public const int BlackPalaceMaxX = 5;
        public const int BlackPalaceMinY = 7;
        public const int BlackPalaceMaxY = 9;
        
        // 棋盘数组 - 存储棋子ID，空位为null
        private string[,] board;
        
        // 棋子字典
        private Dictionary<string, ChessPieceData> pieces;
        
        private void Awake()
        {
            Instance = this;
            board = new string[Width, Height];
            pieces = new Dictionary<string, ChessPieceData>();
        }

        /// <summary>
        /// 放置棋子到棋盘
        /// </summary>
        public bool PlacePiece(ChessPieceData piece, int x, int y)
        {
            if (!IsValidPosition(x, y)) return false;
            if (board[x, y] != null) return false;
            
            piece.x = x;
            piece.y = y;
            board[x, y] = piece.id;
            
            if (!pieces.ContainsKey(piece.id))
            {
                pieces.Add(piece.id, piece);
            }
            
            return true;
        }

        /// <summary>
        /// 移动棋子
        /// </summary>
        public bool MovePiece(string pieceId, int newX, int newY)
        {
            if (!pieces.ContainsKey(pieceId)) return false;
            
            var piece = pieces[pieceId];
            int oldX = piece.x;
            int oldY = piece.y;
            
            if (!IsValidPosition(newX, newY)) return false;
            
            // 清除原位置
            if (oldX >= 0 && oldX < Width && oldY >= 0 && oldY < Height)
            {
                board[oldX, oldY] = null;
            }
            
            // 吃子处理
            if (board[newX, newY] != null)
            {
                string eatenId = board[newX, newY];
                EatPiece(eatenId);
            }
            
            // 放置到新位置
            board[newX, newY] = pieceId;
            piece.x = newX;
            piece.y = newY;
            
            return true;
        }

        /// <summary>
        /// 吃子
        /// </summary>
        public void EatPiece(string pieceId)
        {
            if (pieces.ContainsKey(pieceId))
            {
                pieces.Remove(pieceId);
            }
        }

        /// <summary>
        /// 获取位置的棋子
        /// </summary>
        public ChessPieceData GetPieceAt(int x, int y)
        {
            if (!IsValidPosition(x, y)) return null;
            
            string pieceId = board[x, y];
            if (pieceId == null) return null;
            
            return pieces.ContainsKey(pieceId) ? pieces[pieceId] : null;
        }

        /// <summary>
        /// 获取位置的棋子ID
        /// </summary>
        public string GetPieceIdAt(int x, int y)
        {
            if (!IsValidPosition(x, y)) return null;
            return board[x, y];
        }

        /// <summary>
        /// 验证位置是否有效
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// 检查是否在己方九宫（帅/士）
        /// </summary>
        public bool IsInPalace(int x, int y, bool isRed)
        {
            if (isRed)
            {
                return x >= RedPalaceMinX && x <= RedPalaceMaxX &&
                       y >= RedPalaceMinY && y <= RedPalaceMaxY;
            }
            else
            {
                return x >= BlackPalaceMinX && x <= BlackPalaceMaxX &&
                       y >= BlackPalaceMinY && y <= BlackPalaceMaxY;
            }
        }

        /// <summary>
        /// 检查是否过河
        /// </summary>
        public bool IsCrossedRiver(int y, bool isRed)
        {
            if (isRed)
            {
                return y > RiverRow;
            }
            else
            {
                return y < RiverRow;
            }
        }

        /// <summary>
        /// 清空棋盘
        /// </summary>
        public void ClearBoard()
        {
            board = new string[Width, Height];
            pieces.Clear();
        }

        /// <summary>
        /// 获取所有棋子
        /// </summary>
        public List<ChessPieceData> GetAllPieces()
        {
            return new List<ChessPieceData>(pieces.Values);
        }

        /// <summary>
        /// 获取我方所有棋子
        /// </summary>
        public List<ChessPieceData> GetPiecesByOwner(bool isRed)
        {
            List<ChessPieceData> result = new List<ChessPieceData>();
            foreach (var piece in pieces.Values)
            {
                // 这里可以添加owner属性
                result.Add(piece);
            }
            return result;
        }

        /// <summary>
        /// 打印棋盘（调试用）
        /// </summary>
        public void DebugPrintBoard()
        {
            string output = "Chess Board:\n";
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    string pieceId = board[x, y];
                    if (pieceId == null)
                    {
                        output += "[  ] ";
                    }
                    else if (pieces.ContainsKey(pieceId))
                    {
                        output += $"[{pieces[pieceId].type.ToString().Substring(0, 2)}] ";
                    }
                    else
                    {
                        output += "[??] ";
                    }
                }
                output += "\n";
            }
            Debug.Log(output);
        }
    }
}

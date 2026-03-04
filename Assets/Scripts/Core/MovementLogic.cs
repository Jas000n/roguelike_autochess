using System.Collections.Generic;
using UnityEngine;
using DragonChessLegends.Core;

namespace DragonChessLegends.Core
{
    /// <summary>
    /// 棋子移动逻辑
    /// </summary>
    public static class MovementLogic
    {
        /// <summary>
        /// 获取棋子所有合法移动位置
        /// </summary>
        public static List<Vector2Int> GetValidMoves(ChessPieceData piece, BoardManager board)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            if (piece == null) return moves;

            switch (piece.type)
            {
                case PieceType.General:
                    GetGeneralMoves(piece, board, moves);
                    break;
                case PieceType.Chariot:
                    GetChariotMoves(piece, board, moves);
                    break;
                case PieceType.Horse:
                    GetHorseMoves(piece, board, moves);
                    break;
                case PieceType.Cannon:
                    GetCannonMoves(piece, board, moves);
                    break;
                case PieceType.Elephant:
                    GetElephantMoves(piece, board, moves);
                    break;
                case PieceType.Guard:
                    GetGuardMoves(piece, board, moves);
                    break;
                case PieceType.Soldier:
                    GetSoldierMoves(piece, board, moves);
                    break;
            }

            return moves;
        }

        /// <summary>
        /// 获取棋子所有合法攻击位置
        /// </summary>
        public static List<Vector2Int> GetValidAttacks(ChessPieceData piece, BoardManager board)
        {
            List<Vector2Int> attacks = new List<Vector2Int>();
            
            if (piece == null) return attacks;

            switch (piece.type)
            {
                case PieceType.General:
                    GetGeneralMoves(piece, board, attacks, true);
                    break;
                case PieceType.Chariot:
                    GetChariotAttacks(piece, board, attacks);
                    break;
                case PieceType.Horse:
                    GetHorseAttacks(piece, board, attacks);
                    break;
                case PieceType.Cannon:
                    GetCannonAttacks(piece, board, attacks);
                    break;
                case PieceType.Elephant:
                    GetElephantAttacks(piece, board, attacks);
                    break;
                case PieceType.Guard:
                    GetGuardMoves(piece, board, attacks, true);
                    break;
                case PieceType.Soldier:
                    GetSoldierAttacks(piece, board, attacks);
                    break;
            }

            return attacks;
        }

        // ==================== 帅 ====================
        // 九宫内每格上下左右移动
        private static void GetGeneralMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves, bool isAttack = false)
        {
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
            bool isRed = piece.y < BoardManager.RiverRow;

            for (int i = 0; i < 4; i++)
            {
                int newX = piece.x + directions[i, 0];
                int newY = piece.y + directions[i, 1];

                if (board.IsValidPosition(newX, newY) && board.IsInPalace(newX, newY, isRed))
                {
                    if (isAttack || board.GetPieceIdAt(newX, newY) == null)
                    {
                        moves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // ==================== 车 ====================
        // 直线任意格移动
        private static void GetChariotMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves)
        {
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                for (int i = 1; i < 10; i++)
                {
                    int newX = piece.x + dx * i;
                    int newY = piece.y + dy * i;

                    if (!board.IsValidPosition(newX, newY)) break;

                    if (board.GetPieceIdAt(newX, newY) == null)
                    {
                        moves.Add(new Vector2Int(newX, newY));
                    }
                    else
                    {
                        break; // 有棋子阻挡
                    }
                }
            }
        }

        // 车可以吃直线上的敌人
        private static void GetChariotAttacks(ChessPieceData piece, BoardManager board, List<Vector2Int> attacks)
        {
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                for (int i = 1; i < 10; i++)
                {
                    int newX = piece.x + dx * i;
                    int newY = piece.y + dy * i;

                    if (!board.IsValidPosition(newX, newY)) break;

                    string pieceId = board.GetPieceIdAt(newX, newY);
                    if (pieceId != null)
                    {
                        attacks.Add(new Vector2Int(newX, newY));
                        break;
                    }
                }
            }
        }

        // ==================== 马 ====================
        // 日字形（L型），简化版无蹩马腿
        private static void GetHorseMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves)
        {
            // 马走"日" - 8个方向
            int[,] offsets = {
                { -2, -1 }, { -2, 1 },
                { -1, -2 }, { -1, 2 },
                { 1, -2 }, { 1, 2 },
                { 2, -1 }, { 2, 1 }
            };

            for (int i = 0; i < 8; i++)
            {
                int newX = piece.x + offsets[i, 0];
                int newY = piece.y + offsets[i, 1];

                if (board.IsValidPosition(newX, newY) && board.GetPieceIdAt(newX, newY) == null)
                {
                    moves.Add(new Vector2Int(newX, newY));
                }
            }
        }

        private static void GetHorseAttacks(ChessPieceData piece, BoardManager board, List<Vector2Int> attacks)
        {
            int[,] offsets = {
                { -2, -1 }, { -2, 1 },
                { -1, -2 }, { -1, 2 },
                { 1, -2 }, { 1, 2 },
                { 2, -1 }, { 2, 1 }
            };

            for (int i = 0; i < 8; i++)
            {
                int newX = piece.x + offsets[i, 0];
                int newY = piece.y + offsets[i, 1];

                if (board.IsValidPosition(newX, newY))
                {
                    string pieceId = board.GetPieceIdAt(newX, newY);
                    if (pieceId != null)
                    {
                        attacks.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // ==================== 炮 ====================
        // 直线移动，简化版：直接移动
        private static void GetCannonMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves)
        {
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                for (int i = 1; i < 10; i++)
                {
                    int newX = piece.x + dx * i;
                    int newY = piece.y + dy * i;

                    if (!board.IsValidPosition(newX, newY)) break;

                    if (board.GetPieceIdAt(newX, newY) == null)
                    {
                        moves.Add(new Vector2Int(newX, newY));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // 炮隔子打 - 简化版：可以攻击直线上的敌人
        private static void GetCannonAttacks(ChessPieceData piece, BoardManager board, List<Vector2Int> attacks)
        {
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int d = 0; d < 4; d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];

                for (int i = 1; i < 10; i++)
                {
                    int newX = piece.x + dx * i;
                    int newY = piece.y + dy * i;

                    if (!board.IsValidPosition(newX, newY)) break;

                    string pieceId = board.GetPieceIdAt(newX, newY);
                    if (pieceId != null)
                    {
                        attacks.Add(new Vector2Int(newX, newY));
                        break;
                    }
                }
            }
        }

        // ==================== 象 ====================
        // 田字对角，简化版可过河
        private static void GetElephantMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves)
        {
            int[,] offsets = { { -2, -2 }, { -2, 2 }, { 2, -2 }, { 2, 2 } };

            for (int i = 0; i < 4; i++)
            {
                int newX = piece.x + offsets[i, 0];
                int newY = piece.y + offsets[i, 1];

                if (board.IsValidPosition(newX, newY) && board.GetPieceIdAt(newX, newY) == null)
                {
                    moves.Add(new Vector2Int(newX, newY));
                }
            }
        }

        private static void GetElephantAttacks(ChessPieceData piece, BoardManager board, List<Vector2Int> attacks)
        {
            int[,] offsets = { { -2, -2 }, { -2, 2 }, { 2, -2 }, { 2, 2 } };

            for (int i = 0; i < 4; i++)
            {
                int newX = piece.x + offsets[i, 0];
                int newY = piece.y + offsets[i, 1];

                if (board.IsValidPosition(newX, newY))
                {
                    string pieceId = board.GetPieceIdAt(newX, newY);
                    if (pieceId != null)
                    {
                        attacks.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // ==================== 士 ====================
        // 九宫内斜走
        private static void GetGuardMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves, bool isAttack = false)
        {
            int[,] offsets = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
            bool isRed = piece.y < BoardManager.RiverRow;

            for (int i = 0; i < 4; i++)
            {
                int newX = piece.x + offsets[i, 0];
                int newY = piece.y + offsets[i, 1];

                if (board.IsValidPosition(newX, newY) && board.IsInPalace(newX, newY, isRed))
                {
                    if (isAttack || board.GetPieceIdAt(newX, newY) == null)
                    {
                        moves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // ==================== 兵 ====================
        // 只能向前，过河后可横走
        private static void GetSoldierMoves(ChessPieceData piece, BoardManager board, List<Vector2Int> moves)
        {
            bool isRed = piece.y >= BoardManager.RiverRow; // 红色在上半部分
            bool crossedRiver = board.IsCrossedRiver(piece.y, isRed);

            if (isRed)
            {
                // 红方向上（y减小）
                if (board.IsValidPosition(piece.x, piece.y - 1) && board.GetPieceIdAt(piece.x, piece.y - 1) == null)
                {
                    moves.Add(new Vector2Int(piece.x, piece.y - 1));
                }

                if (crossedRiver)
                {
                    // 过河后可左右
                    if (board.IsValidPosition(piece.x - 1, piece.y) && board.GetPieceIdAt(piece.x - 1, piece.y) == null)
                        moves.Add(new Vector2Int(piece.x - 1, piece.y));
                    if (board.IsValidPosition(piece.x + 1, piece.y) && board.GetPieceIdAt(piece.x + 1, piece.y) == null)
                        moves.Add(new Vector2Int(piece.x + 1, piece.y));
                }
            }
            else
            {
                // 黑方向下（y增大）
                if (board.IsValidPosition(piece.x, piece.y + 1) && board.GetPieceIdAt(piece.x, piece.y + 1) == null)
                {
                    moves.Add(new Vector2Int(piece.x, piece.y + 1));
                }

                if (crossedRiver)
                {
                    if (board.IsValidPosition(piece.x - 1, piece.y) && board.GetPieceIdAt(piece.x - 1, piece.y) == null)
                        moves.Add(new Vector2Int(piece.x - 1, piece.y));
                    if (board.IsValidPosition(piece.x + 1, piece.y) && board.GetPieceIdAt(piece.x + 1, piece.y) == null)
                        moves.Add(new Vector2Int(piece.x + 1, piece.y));
                }
            }
        }

        private static void GetSoldierAttacks(ChessPieceData piece, BoardManager board, List<Vector2Int> attacks)
        {
            bool isRed = piece.y >= BoardManager.RiverRow;
            bool crossedRiver = board.IsCrossedRiver(piece.y, isRed);

            if (isRed)
            {
                if (board.IsValidPosition(piece.x, piece.y - 1))
                    CheckAndAddAttack(piece, board, piece.x, piece.y - 1, attacks);

                if (crossedRiver)
                {
                    CheckAndAddAttack(piece, board, piece.x - 1, piece.y, attacks);
                    CheckAndAddAttack(piece, board, piece.x + 1, piece.y, attacks);
                }
            }
            else
            {
                if (board.IsValidPosition(piece.x, piece.y + 1))
                    CheckAndAddAttack(piece, board, piece.x, piece.y + 1, attacks);

                if (crossedRiver)
                {
                    CheckAndAddAttack(piece, board, piece.x - 1, piece.y, attacks);
                    CheckAndAddAttack(piece, board, piece.x + 1, piece.y, attacks);
                }
            }
        }

        private static void CheckAndAddAttack(ChessPieceData piece, BoardManager board, int x, int y, List<Vector2Int> attacks)
        {
            string pieceId = board.GetPieceIdAt(x, y);
            if (pieceId != null)
            {
                attacks.Add(new Vector2Int(x, y));
            }
        }
    }
}

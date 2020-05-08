﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Server.Configuration;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared;
using WelcomeTo.Shared.Enumerations;

namespace WelcomeTo.Server.Services
{
    public interface IGameBuilder
    {
        public Game Build(string id, string name);

        public Board StartingBoard { get; }

        public ScoreSheet StartingScoreSheet { get; }
    }

    public class GameBuilder : IGameBuilder
    {
        private readonly Dictionary<CardType, List<int>> _cardDistribution;
        private readonly Dictionary<StreetPosition, List<bool>> _poolPositions;
        private readonly Dictionary<StreetPosition, List<int>> _parkPoints;

        private readonly List<CityPlan> _cityPlans1 = new List<CityPlan>();
        private readonly List<CityPlan> _cityPlans2 = new List<CityPlan>();
        private readonly List<CityPlan> _cityPlans3 = new List<CityPlan>();
        private readonly Random _random = new Random();

        public GameBuilder(IOptions<ApplicationOptions> options)
        {
            _cardDistribution = options.Value.CardDistribution;
            _poolPositions = options.Value.PoolPositions;
            _parkPoints = options.Value.ParkPoints;
            foreach (var cityPlan in options.Value.CityPlans)
            {
                switch (cityPlan.Type)
                {
                    case PlanType.No1:
                        _cityPlans1.Add(cityPlan);
                        break;
                    case PlanType.No2:
                        _cityPlans2.Add(cityPlan);
                        break;
                    case PlanType.No3:
                        _cityPlans3.Add(cityPlan);
                        break;
                }
            }
        }

        public Game Build(string id, string name)
        {
            return new Game
            {
                Id = new Guid(id),
                Name = name ?? "Unnamed Game",
                Players = new List<Player> { new Player { Name = "Host", IsHost = true, Board = StartingBoard, ScoreSheet = StartingScoreSheet } },
                GameDeck = GetGameDeck(),
                CurrentTurn = new Turn(),
                Plans = new CityPlans
                {
                    Plan1 = _cityPlans1[_random.Next(_cityPlans1.Count)],
                    Plan2 = _cityPlans2[_random.Next(_cityPlans2.Count)],
                    Plan3 = _cityPlans3[_random.Next(_cityPlans3.Count)]
                }
            };
        }

        public Board StartingBoard => new Board
        {
            TopStreet = GetStreet(StreetPosition.Top),
            MiddleStreet = GetStreet(StreetPosition.Middle),
            BottomStreet = GetStreet(StreetPosition.Bottom)
        };

        public ScoreSheet StartingScoreSheet => new ScoreSheet { RealEstateValueTableCell = new List<RealEstateValueTableCell>() };

        private GameDeck GetGameDeck()
        {
            var deck = new GameDeck()
            {
                Deck1 = new Stack<Card>(),
                Deck2 = new Stack<Card>(),
                Deck3 = new Stack<Card>(),
                Discard1 = new Stack<Card>(),
                Discard2 = new Stack<Card>(),
                Discard3 = new Stack<Card>()
            };

            var cards = _cardDistribution.SelectMany(x => x.Value.Select(number => new Card { Type = x.Key, HouseNumber = number })).Shuffle();
            cards.Distribute(deck.Deck1, deck.Deck2, deck.Deck3);
            return deck;
        }

        private Street GetStreet(StreetPosition position)
        {
            return new Street
            {
                Position = position,
                ParkPoints = _parkPoints[position],
                Houses = _poolPositions[position].Select(hasPool => new House { Pool = hasPool ? PoolType.Unbuilt : PoolType.None }).ToList()
            };
        }
    }
}
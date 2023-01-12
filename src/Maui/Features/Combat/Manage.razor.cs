﻿using DMPowerTools.Core.Features.Combat;
using DMPowerTools.Core.Models;

namespace DMPowerTools.Maui.Features.Combat;

public partial class Manage : IDisposable
{
    [Inject] public IMediator Mediator { get; set; } = null!;

    private readonly CancellationTokenSource _cts = new();
    private ManageCombatQueryResponse _response;
    private List<InitiatedCreature> _initiatedCreatures = new();
    private InitiatedCreature? _clickedCreature;
    private string _selectedCreatureName;
    private bool _creatureDetailsOpen;
    private InitiatedCreature _activeCreature;
    private int _activeCreatureIndex;

    protected override async Task OnInitializedAsync()
    {
        _response = await Mediator.Send(new ManageCombatQuery(), _cts.Token);
    }

    private Task<IEnumerable<string>> Filter(string searchValue)
    {
        var allCreatures = _response.Creatures.Select(n => n.Name);

        if (string.IsNullOrEmpty(searchValue))
        {
            return Task.FromResult(allCreatures);
        }

        var lowerCaseSearchTerm = searchValue.Trim().ToLower();

        return Task.FromResult(allCreatures
            .Where(name => name.Contains(lowerCaseSearchTerm, StringComparison.InvariantCultureIgnoreCase)));
    }

    public void InitiateCreature()
    {
        if (_selectedCreatureName == string.Empty || _selectedCreatureName is null) return;

        var selectedCreature = _response.Creatures.First(c => c.Name == _selectedCreatureName);

        _initiatedCreatures.Add(new InitiatedCreature(0, selectedCreature));
    }

    public void InitiativeRoll()
    {
        foreach (InitiatedCreature a in _initiatedCreatures)
        {
            a.InitiativeRoll = a.Creature.RollInitiative();
        }

        _initiatedCreatures = _initiatedCreatures.OrderByDescending(x => x.InitiativeRoll).ToList();

        if (_initiatedCreatures.Any()) _activeCreature = _initiatedCreatures.First();
        _activeCreatureIndex = 0;
    }

    public void EndTurn()
    {
        if (_activeCreatureIndex < _initiatedCreatures.Count() - 1)
        {
            _activeCreatureIndex++;
        }
        else
        {
            _activeCreatureIndex = 0;
        }

        _activeCreature = _initiatedCreatures[_activeCreatureIndex];
    }

    void OpenDrawer(InitiatedCreature creature)
    {
        _clickedCreature = creature;
        _creatureDetailsOpen = true;
    }
    void CloseDrawer()
    {
        _creatureDetailsOpen = false;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    public class InitiatedCreature
    {
        public InitiatedCreature(int initiativeRoll, ManageCombatQueryResponse.Creature creature)
        {
            InitiativeRoll = initiativeRoll;
            Creature = creature;
            HitPoints = creature.RollHitPoints();
        }

        public int InitiativeRoll { get; set; }
        public ManageCombatQueryResponse.Creature Creature { get; set; }
        public int HitPoints { get; set; }
    }
}
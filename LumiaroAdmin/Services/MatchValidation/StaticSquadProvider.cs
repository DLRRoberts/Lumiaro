using System.Collections.Immutable;
using Lumiaro.MatchValidation.Models;

namespace Lumiaro.MatchValidation.Services;

/// <summary>
/// Placeholder squad provider. Returns the Bayern/Madrid squads from the
/// HTML prototype regardless of the match ID passed in. Replace with a real
/// HTTP-backed implementation that calls into the squad-management API.
/// </summary>
public sealed class StaticSquadProvider : ISquadProvider
{
    public Task<(Squad Home, Squad Away)> GetSquadsAsync(Guid matchId, CancellationToken ct = default)
    {
        var home = BuildBayern();
        var away = BuildMadrid();
        return Task.FromResult((home, away));
    }

    private static Squad BuildBayern()
    {
        var team = new Team(TeamKey.Home, "FC Bayern München");
        var xi = ImmutableArray.Create(
            new Player(1,  "Manuel Neuer"),       new Player(2,  "Dayot Upamecano"),
            new Player(4,  "Jonathan Tah"),       new Player(6,  "Joshua Kimmich"),
            new Player(7,  "Serge Gnabry"),       new Player(9,  "Harry Kane"),
            new Player(14, "Luis Díaz"),          new Player(17, "Michael Olise"),
            new Player(27, "Konrad Laimer"),      new Player(44, "Josip Stanišić"),
            new Player(45, "Aleksandar Pavlović"));
        var subs = ImmutableArray.Create(
            new Player(3,  "Minjae Kim"),         new Player(8,  "Leon Goretzka"),
            new Player(10, "Jamal Musiala"),      new Player(11, "Nicolas Jackson"),
            new Player(19, "Alphonso Davies"),    new Player(21, "Hiroki Ito"),
            new Player(22, "Raphaël Guerreiro"),  new Player(37, "Leonard Prescott"),
            new Player(40, "Jonas Urbig"));
        return new Squad(team, xi, subs);
    }

    private static Squad BuildMadrid()
    {
        var team = new Team(TeamKey.Away, "Real Madrid C.F.");
        var xi = ImmutableArray.Create(
            new Player(13, "Andriy Lunin"),           new Player(3,  "Éder Militão"),
            new Player(5,  "Jude Bellingham"),        new Player(7,  "Vinícius Júnior"),
            new Player(8,  "Federico Valverde"),      new Player(10, "Kylian Mbappé"),
            new Player(12, "Trent Alexander-Arnold"), new Player(15, "Arda Güler"),
            new Player(21, "Brahim Díaz"),            new Player(22, "Antonio Rüdiger"),
            new Player(23, "Ferland Mendy"));
        var subs = ImmutableArray.Create(
            new Player(2,  "Dani Carvajal"),      new Player(4,  "David Alaba"),
            new Player(6,  "Eduardo Camavinga"),  new Player(16, "Gonzalo"),
            new Player(18, "Álvaro Carreras"),    new Player(19, "Dani Ceballos"),
            new Player(20, "Fran García"),        new Player(24, "Dean Huijsen"),
            new Player(26, "Fran González"),      new Player(29, "Javier Navarro"),
            new Player(30, "Franco Mastantuono"), new Player(45, "Thiago Pitarch"));
        return new Squad(team, xi, subs);
    }
}

using Cysharp.Threading.Tasks;

namespace MageSimulator.Scripts.SpellCast
{
    public interface ISpellCaster
    {
        UniTask<SpellCastingEvaluate> Evaluate(Spell spell);
    }
}
using Lumiaro.MatchValidation.Models;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Components.EditPanels;

public partial class TimingEditPanel : ComponentBase
{
    [Parameter, EditorRequired] public TimingEvent Event { get; set; } = default!;
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<TimingEvent> OnConfirm { get; set; }

    private string _realTimeText = string.Empty;
    private bool   _hasError;

    private string TimeInputClass => _hasError ? "ep-realtime field-error" : "ep-realtime";

    protected override void OnParametersSet()
    {
        _realTimeText = Event.RealTime.ToString("HH:mm:ss");
    }

    private void OnRealTimeChanged(ChangeEventArgs e)
    {
        _realTimeText = e.Value?.ToString() ?? string.Empty;
        _hasError = false;
    }

    private async Task HandleConfirm()
    {
        if (!TimeOnly.TryParse(_realTimeText, out var parsed))
        {
            _hasError = true;
            return;
        }

        var updated = Event with { RealTime = parsed, Source = EventSource.Correction };
        await OnConfirm.InvokeAsync(updated);
    }
}

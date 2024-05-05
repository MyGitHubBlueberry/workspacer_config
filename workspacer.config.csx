#r "D:\Programs\workspacer\workspacer.Shared.dll"
#r "D:\Programs\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "D:\Programs\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "D:\Programs\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"
// #r "C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"

using System;
using System.Collections.Generic;
using System.Linq;
using workspacer;
using workspacer.Bar;
using workspacer.Bar.Widgets;
// using workspacer.Gap;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;

Action<IConfigContext> doConfig = (context) =>
{
    var fontName = "CaskaydiaMono Nerd Font";

    context.AddBar(new BarPluginConfig()
    {
        // FontSize = fontSize,
        // BarHeight = barHeight,
        FontName = fontName,
        // DefaultWidgetBackground = background,
        LeftWidgets = () => new IBarWidget[]
        {
            new WorkspaceWidget(),
            new TitleWidget() {
                IsShortTitle = true,
                NoWindowMessage = "",
            },
        },
        RightWidgets = () => new IBarWidget[]
        {
            new TimeWidget(1000, "HH:mm:ss dd MMM yyyy                                                                                                                                         "),
            // new CpuPerformanceWidget(), // not working
            // new MemoryPerformanceWidget(), // not working
            new ActiveLayoutWidget(),
        },
    });

    context.AddFocusIndicator();

    /* Action menu */
    var actionMenu = context.AddActionMenu(new ActionMenuPluginConfig()
    {
        RegisterKeybind = false,
        // MenuHeight = barHeight,
        // FontSize = fontSize,
        FontName = fontName,
        // Background = background,
    });

    /* Action menu builder */
    Func<ActionMenuItemBuilder> createActionMenuBuilder = () =>
    {
        var menuBuilder = actionMenu.Create();

        // Switch to workspace
        menuBuilder.AddMenu("switch", () =>
        {
            var workspaceMenu = actionMenu.Create();
            var monitor = context.MonitorContainer.FocusedMonitor;
            var workspaces = context.WorkspaceContainer.GetWorkspaces(monitor);

            Func<int, Action> createChildMenu = (workspaceIndex) => () =>
            {
                context.Workspaces.SwitchMonitorToWorkspace(monitor.Index, workspaceIndex);
            };

            int workspaceIndex = 0;
            foreach (var workspace in workspaces)
            {
                workspaceMenu.Add(workspace.Name, createChildMenu(workspaceIndex));
                workspaceIndex++;
            }

            return workspaceMenu;
        });

        // Move window to workspace
        menuBuilder.AddMenu("move", () =>
        {
            var moveMenu = actionMenu.Create();
            var focusedWorkspace = context.Workspaces.FocusedWorkspace;

            var workspaces = context.WorkspaceContainer.GetWorkspaces(focusedWorkspace).ToArray();
            Func<int, Action> createChildMenu = (index) => () => { context.Workspaces.MoveFocusedWindowToWorkspace(index); };

            for (int i = 0; i < workspaces.Length; i++)
            {
                moveMenu.Add(workspaces[i].Name, createChildMenu(i));
            }

            return moveMenu;
        });

        // Rename workspace
        menuBuilder.AddFreeForm("rename", (name) =>
        {
            context.Workspaces.FocusedWorkspace.Name = name;
        });

        // Create workspace
        menuBuilder.AddFreeForm("create workspace", (name) =>
        {
            context.WorkspaceContainer.CreateWorkspace(name);
        });

        // Delete focused workspace
        menuBuilder.Add("close", () =>
        {
            context.WorkspaceContainer.RemoveWorkspace(context.Workspaces.FocusedWorkspace);
        });

        // Workspacer
        menuBuilder.Add("toggle keybind helper", () => context.Keybinds.ShowKeybindDialog());
        menuBuilder.Add("toggle enabled", () => context.Enabled = !context.Enabled);
        menuBuilder.Add("restart", () => context.Restart());
        menuBuilder.Add("quit", () => context.Quit());

        return menuBuilder;
    };

    var actionMenuBuilder = createActionMenuBuilder();

    /* Keybindings */
    Action setKeybindings = () =>
    {
        KeyModifiers winShift = KeyModifiers.Win | KeyModifiers.Shift;
        KeyModifiers winCtrl = KeyModifiers.Win | KeyModifiers.Control;
        KeyModifiers win = KeyModifiers.Win;
        KeyModifiers alt = KeyModifiers.Alt;

        IKeybindManager manager = context.Keybinds;

        var workspaces = context.Workspaces;

        manager.UnsubscribeAll();
        manager.Subscribe(alt, Keys.Space,
                () => workspaces.FocusedWorkspace.NextLayoutEngine(), "next layout");

        manager.Subscribe(alt, Keys.OemOpenBrackets, 
                () => workspaces.SwitchToPreviousWorkspace(), "switch to previous workspace");
        manager.Subscribe(alt, Keys.OemCloseBrackets, 
                () => workspaces.SwitchToNextWorkspace(), "switch to next workspace");

        manager.Subscribe(win, Keys.H, 
                () => workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "shrink primary area");
        manager.Subscribe(win, Keys.L, 
                () => workspaces.FocusedWorkspace.ExpandPrimaryArea(), "expand primary area");
        manager.Subscribe(win, Keys.J,
                () => workspaces.FocusedWorkspace.FocusNextWindow(), "focus next window");
        manager.Subscribe(win, Keys.K,
                () => workspaces.FocusedWorkspace.FocusPreviousWindow(), "focus previous window");

        manager.Subscribe(win, Keys.I, 
                () => workspaces.FocusedWorkspace.SwapFocusAndNextWindow(), "swap focus and next window");
        manager.Subscribe(win, Keys.O, 
                () => workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow(), "swap focus and previous window");

        // move focused window to 1,2,3,4 workspace
        manager.Subscribe(win, Keys.D1, 
                () => workspaces.MoveFocusedWindowToWorkspace(0), "switch focused window to workspace 1");
        manager.Subscribe(win, Keys.D2, 
                () => workspaces.MoveFocusedWindowToWorkspace(1), "switch focused window to workspace 2");
        manager.Subscribe(win, Keys.D3, 
                () => workspaces.MoveFocusedWindowToWorkspace(2), "switch focused window to workspace 3");
        manager.Subscribe(win, Keys.D4, 
                () => workspaces.MoveFocusedWindowToWorkspace(3), "switch focused window to workspace 4");
        manager.Subscribe(win, Keys.D5, 
                () => workspaces.MoveFocusedWindowToWorkspace(4), "switch focused window to workspace 5");
        manager.Subscribe(win, Keys.D6, 
                () => workspaces.MoveFocusedWindowToWorkspace(5), "switch focused window to workspace 6");
        manager.Subscribe(win, Keys.D7, 
                () => workspaces.MoveFocusedWindowToWorkspace(6), "switch focused window to workspace 7");
        manager.Subscribe(win, Keys.D8, 
                () => workspaces.MoveFocusedWindowToWorkspace(7), "switch focused window to workspace 8");
        manager.Subscribe(win, Keys.D9, 
                () => workspaces.MoveFocusedWindowToWorkspace(8), "switch focused window to workspace 9");
        //
        manager.Subscribe(alt, Keys.D1,
                () => workspaces.SwitchToWorkspace(0), "switch to workspace 1");
        manager.Subscribe(alt, Keys.D2,
                () => workspaces.SwitchToWorkspace(1), "switch to workspace 2");
        manager.Subscribe(alt, Keys.D3,
                () => workspaces.SwitchToWorkspace(2), "switch to workspace 3");
        manager.Subscribe(alt, Keys.D4,
                () => workspaces.SwitchToWorkspace(3), "switch to workspace 4");
        manager.Subscribe(alt, Keys.D5,
                () => workspaces.SwitchToWorkspace(4), "switch to workspace 5");
        manager.Subscribe(alt, Keys.D6,
                () => workspaces.SwitchToWorkspace(5), "switch to workspace 6");
        manager.Subscribe(alt, Keys.D7,
                () => workspaces.SwitchToWorkspace(6), "switch to workspace 7");
        manager.Subscribe(alt, Keys.D8,
                () => workspaces.SwitchToWorkspace(7), "switch to workspace 8");
        manager.Subscribe(alt, Keys.D9,
                () => workspaces.SwitchToWorkspace(8), "switch to workpsace 9");

        // Add, Subtract keys
        // manager.Subscribe(winCtrl, Keys.Add, () => gapPlugin.IncrementInnerGap(), "increment inner gap");
        // manager.Subscribe(winCtrl, Keys.Subtract, () => gapPlugin.DecrementInnerGap(), "decrement inner gap");
        //
        // manager.Subscribe(winShift, Keys.Add, () => gapPlugin.IncrementOuterGap(), "increment outer gap");
        // manager.Subscribe(winShift, Keys.Subtract, () => gapPlugin.DecrementOuterGap(), "decrement outer gap");

        manager.Subscribe(alt, Keys.A, 
                () => actionMenu.ShowMenu(actionMenuBuilder), "show menu");
        manager.Subscribe(alt, Keys.Escape, 
                () => context.Enabled = !context.Enabled, "toggle enabled/disabled");
        manager.Subscribe(alt, Keys.C, 
                () => context.ToggleConsoleWindow(), "toggle console window");
    };
    setKeybindings();

    context.CanMinimizeWindows = true;

    Func<ILayoutEngine[]> defaultLayouts = () => new ILayoutEngine[]
    {
        new TallLayoutEngine(),
        new VertLayoutEngine(),
        new HorzLayoutEngine(),
        new FullLayoutEngine(),
    };

    context.DefaultLayouts = defaultLayouts;

    context.WorkspaceContainer.CreateWorkspaces("Main", "Communication", "Code", "4", "5");

    //shoud move these windows to according workspaces
    context.WindowRouter.AddRoute((window) =>
            window.Title.Contains("Telegram") || 
            window.Title.Contains("Discord") ||
            window.Title.Contains("LinkedIn") ? context.WorkspaceContainer["Communication"] : null);

    context.WindowRouter.AddRoute((window) =>
            window.Title.Contains("Zoom") ? context.WorkspaceContainer["Main"] : null);

    context.WindowRouter.AddRoute((window) =>
            window.Title.Contains("Terminal") || 
            window.Title.Contains("Ubuntu") ||
            window.Title.Contains("Zellij") ? context.WorkspaceContainer["Code"] : null);
};
return doConfig;

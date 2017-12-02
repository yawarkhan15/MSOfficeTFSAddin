﻿
Imports System.IO
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Microsoft.Office.Tools.Ribbon
Imports Microsoft.Win32
Imports MiYABiS
Imports Microsoft.Vbe.Interop

''' <summary>
''' TFSリボン
''' </summary>
''' <remarks></remarks>
Public Class TfsRibbon

    Public Tf As TfExe

    Private _document As IDocument

    Private _customTaskPaneCollection As Microsoft.Office.Tools.CustomTaskPaneCollection

    Private _docsStat As IDictionary(Of MSProject.Project, CommandEnable) = New Dictionary(Of MSProject.Project, CommandEnable)

    Private lockObject As New Object()

#Region " Handles "

#Region " リボン "

    Private Sub TfsWordRibbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load
        Init()
    End Sub

    Private Sub TfsWordRibbon_Close(sender As Object, e As EventArgs) Handles Me.Close

    End Sub

#End Region
#Region " 外部 "

    ''' <summary>
    ''' Visual Studio 起動ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnVS_Click(sender As Object, e As RibbonControlEventArgs) Handles btnVS.Click
        Me.Tf.OpenVisualStudio()
    End Sub

    ''' <summary>
    ''' エクスプローラで開くボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnExplorer_Click(sender As Object, e As RibbonControlEventArgs) Handles btnExplorer.Click
        Me.Tf.OpenExplorer(_document.FullName)
    End Sub

    ''' <summary>
    ''' Web Accessボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnWebAccess_Click(sender As Object, e As RibbonControlEventArgs) Handles btnWebAccess.Click
        Dim url As String
        url = Me.Tf.GetWebAccess()
        If String.IsNullOrEmpty(url) Then
            Return
        End If
        Me.Tf.ExplorerExecute(url)
    End Sub

#End Region
#Region " VBA "

    ''' <summary>
    ''' VBAコードのエクスポート
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnScriptExport_Click(sender As Object, e As RibbonControlEventArgs) Handles btnScriptExport.Click
        If _document.ActiveDocument Is Nothing Then
            Return
        End If

        Dim doc As MSProject.ProjectClass = _document.ActiveDocument
        Dim vbp As VBProject
        Try
            vbp = doc.VBProject
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return
        End Try

        Dim vba As New Vba
        vba.Export(_document, vbp, Tf)
    End Sub

#End Region
#Region " 操作 "

#Region " 最新を取得 "

    ''' <summary>
    ''' 最新を取得（ブック）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnGetItem_Click(sender As Object, e As RibbonControlEventArgs) Handles btnGetItem.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnGetItem_Click)
    End Sub

    ''' <summary>
    ''' 最新を取得（フォルダ）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnGetItemFolder_Click(sender As Object, e As RibbonControlEventArgs) Handles btnGetItemFolder.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnGetItemFolder_Click)
    End Sub

    ''' <summary>
    ''' 最新を取得（ワークスペース）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnGetWorkspace_Click(sender As Object, e As RibbonControlEventArgs) Handles btnGetWorkspace.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnGetWorkspace_Click)
    End Sub

#End Region

    ''' <summary>
    ''' チェックインボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCheckIn_Click(sender As Object, e As RibbonControlEventArgs) Handles btnCheckIn.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnCheckIn_Click)
    End Sub

    ''' <summary>
    ''' チェックアウトボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCheckOut_Click(sender As Object, e As RibbonControlEventArgs) Handles btnCheckOut.Click
        _document.Action.Execute(AddressOf _actionBtnCheckOut_Click)
    End Sub

    ''' <summary>
    ''' 追加ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAdd_Click(sender As Object, e As RibbonControlEventArgs) Handles btnAdd.Click
        _document.Action.Execute(AddressOf _actionBtnAdd_Click)
    End Sub

    ''' <summary>
    ''' 元に戻すボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnUndo_Click(sender As Object, e As RibbonControlEventArgs) Handles btnUndo.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnUndo_Click)
    End Sub

    ''' <summary>
    ''' 名前を変更ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnRename_Click(sender As Object, e As RibbonControlEventArgs) Handles btnRename.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnRename_Click)
    End Sub

    ''' <summary>
    ''' 棚上げボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnShelve_Click(sender As Object, e As RibbonControlEventArgs) Handles btnShelve.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnShelve_Click)
    End Sub

    ''' <summary>
    ''' 棚上げを復元ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnUnshelve_Click(sender As Object, e As RibbonControlEventArgs) Handles btnUnshelve.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnUnshelve_Click)
    End Sub

    ''' <summary>
    ''' コピーして競合の解決
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnResolveByCopy_Click(sender As Object, e As RibbonControlEventArgs) Handles btnResolveByCopy.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnResolveByCopy_Click)
    End Sub

    ''' <summary>
    ''' 競合の解決ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnResolve_Click(sender As Object, e As RibbonControlEventArgs) Handles btnResolve.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnResolve_Click)
    End Sub

    ''' <summary>
    ''' 履歴ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnHistory_Click(sender As Object, e As RibbonControlEventArgs) Handles btnHistory.Click
        _document.Action.Execute(AddressOf _actionBtnHistory_Click)
    End Sub

    ''' <summary>
    ''' 比較ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDifference_Click(sender As Object, e As RibbonControlEventArgs) Handles btnDifference.Click
        _document.Action.ExecuteCloseOpen(AddressOf _actionBtnDifference_Click)
    End Sub

    ''' <summary>
    ''' プロパティボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnInfo_Click(sender As Object, e As RibbonControlEventArgs) Handles btnInfo.Click
        _document.Action.Execute(AddressOf _actionBtnInfo_Click)
    End Sub

#End Region
#Region " ウィンドウ "

    ''' <summary>
    ''' 出力ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnOutputPane_Click(sender As Object, e As RibbonControlEventArgs) Handles btnOutputPane.Click
        Me.Tf.ShowOutputTaskPane(Globals.ThisAddIn)
    End Sub

#End Region

#End Region
#Region " Action "

    ''' <summary>
    ''' 最新を取得（ブック）
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnGetItem_Click(ByVal context As ActionContext)
        _actionGet(context, context.FullName)
    End Sub

    ''' <summary>
    ''' 最新を取得（フォルダ）
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnGetItemFolder_Click(ByVal context As ActionContext)
        _actionGet(context, Path.GetDirectoryName(context.FullName))
    End Sub

    ''' <summary>
    ''' 最新を取得（ワークスペース）
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnGetWorkspace_Click(ByVal context As ActionContext)
        _actionGet(context)
    End Sub

    ''' <summary>
    ''' 最新を取得
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="name"></param>
    ''' <remarks></remarks>
    Private Sub _actionGet(ByVal context As ActionContext, Optional ByVal name As String = "")
        Me.Tf.Get(name)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        context.OutputActive = True
    End Sub

    ''' <summary>
    ''' チェックインボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnCheckIn_Click(ByVal context As ActionContext)
        ' チェックイン
        Me.Tf.CheckIn(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
    End Sub

    ''' <summary>
    ''' チェックアウトボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnCheckOut_Click(ByVal context As ActionContext)
        Me.Tf.CheckOut(_document.Name)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        context.StatusBar = TfCommand.My.Resources.Messages.I002
    End Sub

    ''' <summary>
    ''' 追加ボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnAdd_Click(ByVal context As ActionContext)
        Me.Tf.Add(_document.Name)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        context.StatusBar = TfCommand.My.Resources.Messages.I004
    End Sub

    ''' <summary>
    ''' 元に戻すボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnUndo_Click(ByVal context As ActionContext)
        If MessageBox.Show(TfCommand.My.Resources.Messages.Q001, TfCommand.My.Resources.Messages.MessageBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then
            Return
        End If

        Me.Tf.Undo(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        context.StatusBar = TfCommand.My.Resources.Messages.I003
    End Sub

    ''' <summary>
    ''' 名前を変更ボタンクリック
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub _actionBtnRename_Click(ByVal context As ActionContext)
        Dim sfd As New SaveFileDialog()
        sfd.Title = TfCommand.My.Resources.Messages.Q004
        sfd.FileName = context.Name
        sfd.AddExtension = True
        sfd.DefaultExt = Path.GetExtension(context.Name)
        sfd.InitialDirectory = Path.GetDirectoryName(context.FullName)
        sfd.RestoreDirectory = True
        If sfd.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim filename As String = sfd.FileName
        If Me.Tf.Rename(context.FullName, filename) Then
            context.FullName = filename
        End If
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
    End Sub

    ''' <summary>
    ''' 棚上げボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnShelve_Click(ByVal context As ActionContext)
        Me.Tf.Shelve(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
    End Sub

    ''' <summary>
    ''' 棚上げを復元ボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnUnshelve_Click(ByVal context As ActionContext)
        If MessageBox.Show(TfCommand.My.Resources.Messages.Q003, TfCommand.My.Resources.Messages.MessageBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then
            Return
        End If

        Dim unshelve As Boolean
        unshelve = Me.Tf.Unshelve(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        If Not unshelve Then
            Return
        End If
        context.StatusBar = TfCommand.My.Resources.Messages.I003
    End Sub

    ''' <summary>
    ''' コピーして競合の解決
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnResolveByCopy_Click(ByVal context As ActionContext)
        Const C_NAME As String = "{0}({1})"
        Dim dir As String = Path.GetDirectoryName(context.FullName)
        Dim name As String = context.Name
        Dim nameCopy As String = String.Format(C_NAME, name.Replace(Path.GetExtension(name), ""), Environment.UserName) & Path.GetExtension(name)
        Dim filename As String = Path.Combine(dir, name)
        Dim filename2 As String = Path.Combine(dir, nameCopy)

        ' ブックを別名で保存
        File.Copy(filename, filename2)
        _document.Open(filename2)

        ' 元に戻す
        Dim rc As Boolean
        rc = Me.Tf.Undo(filename)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        If Not rc Then
            Return
        End If
        ' 最新を取得する
        rc = Me.Tf.Get(filename)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        If Not rc Then
            Return
        End If

        _document.Open(filename)
        _document.CompareSideBySideWith(nameCopy)
    End Sub

    ''' <summary>
    ''' 競合の解決ボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnResolve_Click(ByVal context As ActionContext)
        Me.Tf.Resolve(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
    End Sub

    ''' <summary>
    ''' 履歴ボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnHistory_Click(ByVal context As ActionContext)
        Me.Tf.History(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
    End Sub

    ''' <summary>
    ''' 比較ボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnDifference_Click(ByVal context As ActionContext)
        Dim diff As Boolean
        diff = Me.Tf.IsDiff(context.FullName)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError

        If Not context.StandardError.Length.Equals(0) Then
            If context.StandardError.Contains(_document.Name) Then
                context.StatusBar = context.StandardError
            End If
            Return
        End If
        If Not diff Then
            context.StatusBar = TfCommand.My.Resources.Messages.I005
            Return
        End If
        MessageBox.Show(TfCommand.My.Resources.Messages.I001, TfCommand.My.Resources.Messages.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        context.StatusBar = TfCommand.My.Resources.Messages.I001
    End Sub

    ''' <summary>
    ''' プロパティボタンクリック
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Private Sub _actionBtnInfo_Click(ByVal context As ActionContext)
        Me.Tf.Info(context.Name)
        context.StandardOutput = Me.Tf.StandardOutput
        context.StandardError = Me.Tf.StandardError
        context.OutputActive = True
    End Sub

#End Region
#Region " Method "

    Public Sub Init()
        _document = Globals.ThisAddIn

        '_customTaskPaneCollection = Globals.Factory.CreateCustomTaskPaneCollection(Nothing, Nothing, "CustomTaskPanes", "CustomTaskPanes", Me)
        _customTaskPaneCollection = Globals.Factory.CreateCustomTaskPaneCollection(Globals.ThisAddIn.ItemProvider, Globals.ThisAddIn.HostContext, "CustomTaskPanes", "CustomTaskPanes", Me)

        Me.Tf = New TfExe(_customTaskPaneCollection)

        'Me.tbtnOutputPane.Visible = False
        Me.Tf.CreateOutputTaskPane(Globals.ThisAddIn)
    End Sub

    Public Sub Disable()
        Me.groupAction.Visible = False
        Me.groupWindow.Visible = False
        Me.btnWebAccess.Visible = False
    End Sub

    Public Sub SetEnabled(ByVal Wb As Microsoft.Office.Interop.MSProject.Project, ByVal force As Boolean, Optional ByVal groupDisable As Boolean = False)
        SyncLock lockObject

            Dim newDoc As Boolean = False

            If Not _docsStat.ContainsKey(Wb) Then
                _docsStat(Wb) = New CommandEnable
                newDoc = True
            End If
            Dim enables As CommandEnable = _docsStat(Wb)

            If Not force AndAlso Not newDoc Then
                btnCheckIn.Enabled = enables.btnCheckIn
                btnCheckOut.Enabled = enables.btnCheckOut
                btnUndo.Enabled = enables.btnUndo
                btnShelve.Enabled = enables.btnShelve
                btnResolveByCopy.Enabled = enables.btnResolveByCopy

                btnAdd.Enabled = enables.btnAdd
                btnRename.Enabled = enables.btnRename
                btnDifference.Enabled = enables.btnDifference
                btnHistory.Enabled = enables.btnHistory

                btnWebAccess.Visible = enables.btnWebAccess
                groupAction.Visible = enables.groupAction
                groupWindow.Visible = enables.groupWindow

                Return
            End If

            Disable()

            If Wb Is Nothing Then
                Return
            End If
            Dim visible As Boolean
            Me.Tf.WorkingDirectory = Wb.Path
            If String.IsNullOrEmpty(Wb.Path) Then
                visible = False
            Else
                visible = Me.Tf.Workfold(Wb.Name)
            End If

            If Not visible Then
                Disable()
                _setEnabeled(enables)
                Return
            End If

            If Me.Tf.Status(Wb.Name) Then
                Me.btnCheckIn.Enabled = True
                Me.btnCheckOut.Enabled = False
                Me.btnUndo.Enabled = True
                Me.btnShelve.Enabled = True
                Me.btnResolveByCopy.Enabled = True
            Else
                Me.btnCheckIn.Enabled = False
                Me.btnCheckOut.Enabled = True
                Me.btnUndo.Enabled = False
                Me.btnShelve.Enabled = False
                Me.btnResolveByCopy.Enabled = False
            End If
            Me.btnAdd.Enabled = Me.Tf.IsAdd(Wb.Name)
            If Me.btnAdd.Enabled Then
                Me.btnCheckIn.Enabled = False
                Me.btnUndo.Enabled = False
                Me.btnShelve.Enabled = False
                Me.btnRename.Enabled = False
                Me.btnResolveByCopy.Enabled = False
                Me.btnDifference.Enabled = False
                Me.btnHistory.Enabled = False
            Else
                Me.btnRename.Enabled = True
                Me.btnDifference.Enabled = True
                Me.btnHistory.Enabled = True
            End If

            Me.btnWebAccess.Visible = visible
            Me.groupAction.Visible = visible
            Me.groupWindow.Visible = visible

            _setEnabeled(enables)

        End SyncLock
    End Sub

    Public Sub RemoveEnables(ByVal Doc As MSProject.Project)
        _docsStat.Remove(Doc)
    End Sub

    Private Sub _setEnabeled(ByVal enables As CommandEnable)
        enables.btnCheckIn = btnCheckIn.Enabled
        enables.btnCheckOut = btnCheckOut.Enabled
        enables.btnUndo = btnUndo.Enabled
        enables.btnShelve = btnShelve.Enabled
        enables.btnResolveByCopy = btnResolveByCopy.Enabled

        enables.btnAdd = btnAdd.Enabled
        enables.btnRename = btnRename.Enabled
        enables.btnDifference = btnDifference.Enabled
        enables.btnHistory = btnHistory.Enabled

        enables.btnWebAccess = btnWebAccess.Visible
        enables.groupAction = groupAction.Visible
        enables.groupWindow = groupWindow.Visible
    End Sub

    Public Sub AddMessage(ByVal value As String)
        Me.Tf.AddMessage(Globals.ThisAddIn, value)
    End Sub

    Private Sub _error(Optional ByVal standardError As String = "")
        If String.IsNullOrEmpty(standardError) Then
            standardError = Me.Tf.StandardError
        End If
        MessageBox.Show(standardError,
                        TfCommand.My.Resources.Messages.MessageBoxTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation)
        Me.AddMessage(standardError)
    End Sub

#End Region

End Class
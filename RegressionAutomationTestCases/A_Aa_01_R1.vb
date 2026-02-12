
'--------------------------------------------------------------- 
'	(c) 2004 Koninklijke Philips Electronics N.V.
'	All Rights Reserved.
'--------------------------------------------------------------- 

#Region "Header"
Imports System
Imports System.Math
Imports Philips.CIS.Calculations
Imports Philips.CIS.Calculations.Base
#End Region

<Calculation(CalculationType.KeyAdvisory)> Public Class A_Aa_01_R1
    Inherits Philips.CIS.Calculations.Base.Calculation


#Region "Declare your input and output parameters here"


    ' Other input parameters
    <Input()> Public currentFiO2 As New Interventions.assessIntervenCat.RespiratoryIntCat.FiO2Int.FiO2Obs.FiO2Msmt(120, 15)
    <Key()> Public currentPaO2 As New Interventions.assessIntervenCat.labAssessIntervenCat.arterialpO2MsmtInt.arterialpO2MsmtObs.arterialPO2Msmt
    <Input()> Public currentPaCO2 As New Interventions.assessIntervenCat.labAssessIntervenCat.ArterialpCO2Int.ArterialpCO2Obs.ArterialpCO2Num(120, 15)


    'The current Advisory declaration
    <Output()> Public currentAdvisory As New Interventions.otherIntervenCat.AdvisoryInt.advisoryText
    <Output()> Public currentAdvisoryTag As New Interventions.otherIntervenCat.AdvisoryInt.AdvisoryTag
    <Output()> Public target As New Interventions.otherIntervenCat.AdvisoryInt.advisoryTarget
    <Output()> Public severity As New Interventions.otherIntervenCat.AdvisoryInt.advisorySeverity

#End Region


#Region "Define your calculation formula here"

    Overrides Function DoCalculation() As Boolean

        '-----------------------------------------------------------------------------------
        '	RULE SUMMARY
        '	-------------
        '	Calculation:   If (PB*FiO2) -(PaCO2/0.8)-PaO2  > 100  then issue Advisory:
        '                 " A-a gradient {} (> 100), charted @ {}.      "
        '  Refractory:    If [A_Aa_01_R1] was issued in the past 4 hours.
        '  Overridden by: none 
        '-----------------------------------------------------------------------------------
        ' Initial release: 17 Nov 2005
        'Modified:   26 Nov 2005                          
        '
        '--- BEGIN, STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------
        Const idString As String = "[A_Aa_01_R1]" '  string for unique id of the advisory intervention
        Const Version As String = "[26Nov06]" ' Version date
        Dim currentTime As DateTime = DateTime.Now
        Debug.WriteLine(String.Format("[Adv] " + idString.ToString + " Version =" + Version + " @ " + currentTime.ToShortTimeString))


        'Check if HIGHER SEVERITY (MUTUALLY EXCLUSIVE)ADVISORY RULE has been annunciated in the past xxx hours.
        ' Add code here if required

        ' Check if meets criteria of HIGHER SEVERITY (MUTUALLY EXCLUSIVE)ADVISORY RULE. If so, exit
        ' Add code here if required


        '--- END,   STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------

        '	check for required variables
        If (CheckForRequiredValues(currentFiO2) = False) Then
            Debug.WriteLine("[Adv] " + idString.ToString + " no currentFiO2 ")

        Else
            Debug.WriteLine("[Adv] " + idString.ToString + "currentFiO2 = " + currentFiO2.NumericValue.ToString("##.#") + " @ " + currentFiO2.ChartTime.ToLocalTime.ToShortTimeString)
        End If

        If (CheckForRequiredValues(currentPaCO2) = False) Then
            Debug.WriteLine("[Adv] " + idString.ToString + " no currentPaCO2")

        Else
            Debug.WriteLine("[Adv] " + idString.ToString + "currentPaCO2 = " + currentPaCO2.NumericValue.ToString("##.#") + " @ " + currentPaCO2.ChartTime.ToLocalTime.ToShortTimeString)

        End If
        If (CheckForRequiredValues(currentPaO2) = False) Then
            Debug.WriteLine("[Adv] " + idString.ToString + " no currentpaO2")

        Else
            Debug.WriteLine("[Adv] " + idString.ToString + "currentPaO2 = " + currentPaO2.NumericValue.ToString("##.#") + " @ " + currentPaO2.ChartTime.ToLocalTime.ToShortTimeString)

        End If

        If (CheckForRequiredValues(currentFiO2, currentPaO2, currentPaCO2) = False) Then
            Debug.WriteLine("[Adv] " + idString.ToString + "Didn't find all 3 values, exit ")
            Return False
        End If

        '
        '  are input "units of measure" same as  "expected units of measure", i.e: g/L, cm, mmHG etc.? 
        '  if not, convert here to "expected units of measure"
        ' ----------------------------------------------------------------------------------
        Dim currentFiO2_percent As Decimal = currentFiO2.NumericValue / 100 ' stored as percent
        Dim currentPaO2_mmHg As Decimal = currentPaO2.NumericValue  '
        Dim currentPaCO2_mmHg As Decimal = currentPaCO2.NumericValue  ' stored as mmHg
        Debug.WriteLine("[Adv] " + idString.ToString + "current PaO2 = " + currentPaO2_mmHg.ToString("##.#") + " @ " + currentPaO2.ChartTime.ToLocalTime.ToShortTimeString)
        Debug.WriteLine("[Adv] " + idString.ToString + "currentFiO2_percent = " + currentFiO2_percent.ToString("##.#") + " @ " + currentFiO2.ChartTime.ToLocalTime.ToShortTimeString)
        Debug.WriteLine("[Adv] " + idString.ToString + "currentPaCO2 = " + currentPaCO2_mmHg.ToString("##.#") + " @ " + currentPaCO2.ChartTime.ToLocalTime.ToShortTimeString)

        '	Enter the actual advisory rule here
        '-----------------------------------------------------------------------------------
        Dim myString As String = " Alveolar-arterial gradient {0}, charted @ {1}  (High, > {2})."
        Dim BarometricPressure As Decimal = 713  ' 760 - Water vapor pressure (eg 47 mmHg at 37 degrees)
        Dim AaGradient As Decimal
        Dim Threshold1 As Decimal = 100



        AaGradient = (currentFiO2_percent * BarometricPressure) - (currentPaCO2_mmHg / 0.8) - currentPaO2_mmHg

        If AaGradient > Threshold1 Then
            currentAdvisory.TextValue = String.Format(myString, AaGradient.ToString("##.#"), currentPaO2.ChartTime.ToLocalTime.ToShortTimeString, Threshold1.ToString("#"))
            currentAdvisoryTag.TextValue = idString

            Debug.WriteLine("[Adv] " + idString.ToString + "*** advisory message was issued using value = " + currentPaO2.NumericValue.ToString("##.#") + " at " + currentPaO2.ChartTime.ToLocalTime.ToShortTimeString)
            '	Standard set of checks in each Rule
            '-----------------------------------------------------------------------------------

            Me.State.SuppressUntil = DateTime.UtcNow.AddHours(4)
            Me.State.NotificationSustainTime = 240  ' Advisory message persistence in minutes
            target.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisoryTarget.ChartAndOutbound
            severity.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisorySeverity.Low


            Return True
        Else
            Debug.WriteLine("[Adv] " + idString.ToString + " did not meet criteria; exit ")
            Return False

        End If

    End Function

#End Region


#Region "Declare default values for debugging here (optional)"

    Protected Overloads Overrides Sub InitValuesForDebug()
        ' Define default values for  input variables

        ' {InputVariableName} = {value}
        currentFiO2.NumericValue = 50
        currentPaO2.NumericValue = 39.0
        currentPaCO2.NumericValue = 47.0
        ' result  = 258.8

    End Sub

#End Region


End Class
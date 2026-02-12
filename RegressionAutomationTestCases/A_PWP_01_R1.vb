
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

<Calculation(CalculationType.KeyAdvisory)> Public Class A_PWP_01_R1
    Inherits Philips.CIS.Calculations.Base.Calculation


#Region "Declare your input and output parameters here"


    <Key()> Public currentPCWP As New Interventions.assessIntervenCat.cardiovascularIntCat.PCWPInt.PCWPObs.PCWPMsmt

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
        '	------------
        '	Calculation:   If current PCWP > 22 mmHg  then issue Advisory:
        '                 " Pulmonary Capillary Wedge Pressure {}mmHg (> 22)  @ time "
        '                 
        '                 
        '	Refractory:     If [A_PWP_01_R1] was issued in the past 120 minutes.
        '	Overridden by:  none
        '-----------------------------------------------------------------------------------
        ' Initial release: 17 Nov 2005
        'Modified:   26 Nov 2005                          
        '
        '--- BEGIN, STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------
        Const idString As String = "[A_PWP_01_R1]" '  string for unique id of the advisory intervention
        Const Version As String = "[01Dec06]" ' Version date
        Dim currentTime As DateTime = DateTime.Now
        Debug.WriteLine(String.Format("[Adv] " + idString.ToString + " Version =" + Version + " @ " + currentTime.ToShortTimeString))

        '	Standard set of checks in each Rule
        '-----------------------------------------------------------------------------------

        Me.State.SuppressUntil = DateTime.UtcNow.AddHours(4)
        Me.State.NotificationSustainTime = 240  ' Advisory message persistence in minutes
        target.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisoryTarget.ChartAndOutbound



        'Check if HIGHER SEVERITY (MUTUALLY EXCLUSIVE)ADVISORY RULE has been annunciated in the past xxx hours.
        ' Add code here if required

        ' Check if meets criteria of HIGHER SEVERITY (MUTUALLY EXCLUSIVE)ADVISORY RULE. If so, exit
        ' Add code here if required



        '--- END,   STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------

        '	check for required variables
        If (CheckForRequiredValues(currentPCWP) = False) Then
            'Debug.WriteLine("[Adv] " + idString.ToString + " no current values found.")
            Return False
        End If

        '	Enter the actual advisory rule here
        '-----------------------------------------------------------------------------------
        Dim localEventTime As DateTime
        Dim myString As String = " Pulmonary Capillary Wedge Pressure {0} mmHg (> {2} mmHg),  charted @ {1} "

        '-----------------------------------------------------------------------------------
        Dim ThresholdValue1 As Decimal = 22.0
        Dim ThresholdValue2 As Decimal = 0.0
        'Debug.WriteLine("[Adv] " + idString.ToString + " current Care Unit =  " + Me.CurrentCareUnit.ToString)

        'Dim CurrentCareUnit As String = Me.CurrentCareUnit
        'If (InStr(CurrentCareUnit, "MICU", CompareMethod.Text) > 0) Then ThresholdValue = 
        localEventTime = currentPCWP.ChartTime.ToLocalTime
        'Debug.WriteLine("[Adv] " + idString.ToString + " current value = " + currentPCWP.NumericValue.ToString() + " at " + localEventTime.ToShortTimeString)



        If (currentPCWP.NumericValue > ThresholdValue1) Then
            currentAdvisory.TextValue = String.Format(myString, currentPCWP.NumericValue.ToString("0.0"), localEventTime.ToShortTimeString(), ThresholdValue1.ToString("0.0"))
            currentAdvisoryTag.TextValue = idString
            'Debug.WriteLine("[Adv] " + idString.ToString + "*** advisory message was issued using value = " + currentPCWP.NumericValue.ToString() + " at " + localEventTime.ToShortTimeString)
            severity.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisorySeverity.High
            Return True
        Else
            'Debug.WriteLine("[Adv] " + idString.ToString + " did not meet criteria; exit ")
            Return False

        End If


    End Function

#End Region


#Region "Declare default values for debugging here (optional)"

    Protected Overloads Overrides Sub InitValuesForDebug()
        ' Define default values for  input variables

        ' {InputVariableName} = {value}
        currentPCWP.NumericValue = 23




    End Sub

#End Region


End Class
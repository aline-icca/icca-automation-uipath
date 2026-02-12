
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

<Calculation(CalculationType.KeyAdvisory)> Public Class A_FiO2_01_R1
    Inherits Philips.CIS.Calculations.Base.Calculation


#Region "Declare your input and output parameters here"

    <Key()> Public currentFiO2 As New Interventions.assessIntervenCat.RespiratoryIntCat.FiO2Int.FiO2Obs.FiO2Msmt
    <Input()> Public recentFiO2 As New Interventions.assessIntervenCat.RespiratoryIntCat.FiO2Int.FiO2Obs.FiO2Msmt(180, 60)
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
        '	Calculation:   If FiO2 mean  > 60%  for 4 hours  then issue Advisory:
        '                 " FiO2 mean value  {} % (> 60%) for 4 hours, charted @ {} "
        '                 
        '                 
        '	Refractory:     If [A_FiO2_01_R1] was issued in the past 240 minutes.
        '	Overridden by:  none
        '-----------------------------------------------------------------------------------
        ' Initial release: 17 Nov 2005
        'Modified:   26 Nov 2005                          
        '
        '--- BEGIN, STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------
        Const idString As String = "[A_FiO2_01_R1]" '  string for unique id of the advisory intervention
        Const Version As String = "[26Nov06]" ' Version date
        Dim currentTime As DateTime = DateTime.Now
        Debug.WriteLine(String.Format("[Adv] " + idString.ToString + " Version =" + Version + " @ " + currentTime.ToShortTimeString))



        '--- END,   STANDARD SET of RULE SUPPRESSIONS and OVERRIDES
        '-----------------------------------------------------------------------------------

        '	check for required variables
        If (CheckForRequiredValues(currentFiO2, recentFiO2) = False) Then
            Debug.WriteLine("[Adv] " + idString.ToString + " no current values found.")
            Return False
        End If

        '	Enter the actual advisory rule here
        '-----------------------------------------------------------------------------------
        Dim localEventTime As DateTime
        Dim myString As String = " FiO2 mean value  {0} % (> {2}%) for 3 hours, charted @ {1} "

        '-----------------------------------------------------------------------------------
        Dim ThresholdValue1 As Decimal = 60.0
        Dim ThresholdValue2 As Decimal = 0.0
        Debug.WriteLine("[Adv] " + idString.ToString + " current Care Unit =  " + Me.CurrentCareUnit.ToString)

        'Dim CurrentCareUnit As String = Me.CurrentCareUnit
        'If (InStr(CurrentCareUnit, "MICU", CompareMethod.Text) > 0) Then ThresholdValue = 
        localEventTime = currentFiO2.ChartTime.ToLocalTime
        Debug.WriteLine("[Adv] " + idString.ToString + " current value = " + currentFiO2.NumericValue.ToString() + " at " + localEventTime.ToShortTimeString)


        Dim FiO2Meanvalue As Decimal = recentFiO2.Mean
        Dim FiO2Count As Decimal = recentFiO2.Count
        Debug.WriteLine("[Adv] " + idString.ToString + " FiO2Count = " + FiO2Count.ToString("#.#") + " FiO2Meanvalue = " + FiO2Meanvalue.ToString("#.#"))

        If (FiO2Meanvalue > ThresholdValue1) And (FiO2Count = 3) Then
            currentAdvisory.TextValue = String.Format(myString, FiO2Meanvalue.ToString("0.0"), localEventTime.ToShortTimeString(), ThresholdValue1.ToString("0.0"))
            currentAdvisoryTag.TextValue = idString
            Debug.WriteLine("[Adv] " + idString.ToString + "*** advisory message was issued using value = " + FiO2Meanvalue.ToString() + " at " + localEventTime.ToShortTimeString)

            '	Standard set of checks in each Rule
            '-----------------------------------------------------------------------------------

            Me.State.SuppressUntil = DateTime.UtcNow.AddHours(4)
            Me.State.NotificationSustainTime = 180  ' Advisory message persistence in minutes
            severity.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisorySeverity.Medium
            target.ChoiceValue = CatalogItems.Interventions.AdvisoryInt.advisoryTarget.ChartAndOutbound
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
        recentFiO2.NumericValue = 62




    End Sub

#End Region


End Class
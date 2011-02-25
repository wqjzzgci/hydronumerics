Public Class GPS

#Region "Enums"
    Public Enum CardinalDirection
        North
        East
        South
        West
    End Enum

    Public Enum UTMLat
        North = CardinalDirection.North
        South = CardinalDirection.South
    End Enum

    Public Enum CoordinateType
        Latitude
        Longitude
    End Enum

    Public Enum UTMDatum
        WGS84
        NAD83
        GRS80
        WGS72
        Australian1965
        Krasovsky1940
        NorthAmerican1927
        International1924
        Hayford1909
        Clarke1880
        Clarke1866
        Airy1830
        Bessel1841
        Everest1830
    End Enum
#End Region

#Region "Structures"
    Public Structure Coordinate
        Public Degrees As Int32
        Public Minutes As Int32
        Public Seconds As Decimal
        Public Direction As CardinalDirection

        Public Sub New(ByVal Degrees As Int32, ByVal Minutes As Int32, ByVal seconds As Decimal, ByVal Direction As CardinalDirection)
            Me.Degrees = Degrees
            Me.Minutes = Minutes
            Me.Seconds = seconds
            Me.Direction = Direction
        End Sub
        Public Sub New(ByVal Degrees As Decimal, ByVal Type As CoordinateType)
            If Type = CoordinateType.Latitude Then
                If Degrees > 90 OrElse Degrees < -90 Then Exit Sub
                Me.Direction = If(Degrees < 0, CardinalDirection.South, CardinalDirection.North)
            Else
                If Degrees > 180 OrElse Degrees < -180 Then Exit Sub
                Me.Direction = If(Degrees < 0, CardinalDirection.West, CardinalDirection.East)
            End If

            Degrees = Math.Abs(Degrees)
            Me.Degrees = Convert.ToInt32(Math.Truncate(Degrees))
            Me.Minutes = Convert.ToInt32(Math.Truncate((Degrees - Math.Truncate(Degrees)) * 60D))
            Me.Seconds = (Degrees - Math.Truncate(Degrees) - Convert.ToDecimal(Me.Minutes) / 60) * 3600
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}° {1}' {2:f3}"" {3}", Degrees, Minutes, Seconds, Direction)
        End Function

        Private Function FlipDirection(ByVal Direction As CardinalDirection) As CardinalDirection
            Select Case Direction
                Case CardinalDirection.North
                    Return CardinalDirection.South
                Case CardinalDirection.East
                    Return CardinalDirection.West
                Case CardinalDirection.South
                    Return CardinalDirection.North
                Case CardinalDirection.West
                    Return CardinalDirection.East
            End Select
        End Function

        Public Function GetAbsoluteDecimalCoordinate() As Decimal
            Return Convert.ToDecimal(Degrees) + Convert.ToDecimal(Minutes) / 60D + Seconds / 3600D
        End Function

        Public Function GetDecimalCoordinate() As Decimal
            Dim dec As Decimal = Convert.ToDecimal(Degrees) + Convert.ToDecimal(Minutes) / 60D + Seconds / 3600D
            Return If((Direction = CardinalDirection.North OrElse Direction = CardinalDirection.East), dec, -dec)
        End Function
    End Structure
#End Region

#Region "UTM Object"
    Public Class UTM
        Private Const k0 As Double = 0.9996

        Private cooLat As New Coordinate(0, CoordinateType.Latitude)
        Private cooLong As New Coordinate(0, CoordinateType.Longitude)
        Private utmDat As UTMDatum = UTMDatum.WGS84
        Private dblNorthing As Double = 0
        Private dblEasting As Double = 0
        Private utmL As UTMLat = UTMLat.North
        Private intZone As Int32

#Region "Properties"

        Public Property UTMLatitudeHemisphere() As UTMLat
            Get
                Return utmL
            End Get
            Set(ByVal value As UTMLat)
                utmL = value
                GetLatLong()
            End Set
        End Property

        Public Property Datum() As UTMDatum
            Get
                Return utmDat
            End Get
            Set(ByVal value As UTMDatum)
                utmDat = value
            End Set
        End Property

        Public Property Latitude() As Coordinate
            Get
                Return cooLat
            End Get
            Set(ByVal value As Coordinate)
                If value.Direction = CardinalDirection.North OrElse value.Direction = CardinalDirection.South Then
                    cooLat = value
                    utmL = DirectCast(cooLat.Direction, UTMLat)
                    GetUTM()
                End If
            End Set
        End Property

        Public Property Longitude() As Coordinate
            Get
                Return cooLong
            End Get
            Set(ByVal value As Coordinate)
                If value.Direction = CardinalDirection.East OrElse value.Direction = CardinalDirection.West Then
                    cooLong = value
                    GetUTM()
                End If
            End Set
        End Property

        Public Property Zone() As Int32
            Get
                Return intZone
            End Get
            Set(ByVal value As Int32)
                intZone = value
                GetLatLong()
            End Set
        End Property

        Public Property Easting() As Double
            Get
                Return dblEasting
            End Get
            Set(ByVal value As Double)
                dblEasting = value
                GetLatLong()
            End Set
        End Property

        Public Property Northing() As Double
            Get
                Return dblNorthing
            End Get
            Set(ByVal value As Double)
                dblNorthing = value
                GetLatLong()
            End Set
        End Property

#End Region

#Region "Constructors"
        Public Sub New(ByVal Latitude As Coordinate, ByVal Longitude As Coordinate)
            cooLat = Latitude
            cooLong = Longitude
            intZone = GetZone()
            utmL = DirectCast(cooLat.Direction, UTMLat)
            GetUTM()
        End Sub

        Public Sub New(ByVal Northing As Double, ByVal Easting As Double, ByVal Zone As Int32)
            dblNorthing = Northing
            dblEasting = Easting
            intZone = Zone
            GetLatLong()
        End Sub
#End Region

        Public Function GetZone() As Int32
            Dim decLongAbs As Decimal = cooLong.GetDecimalCoordinate
            If cooLong.Direction = CardinalDirection.West Then
                Return Convert.ToInt32(Math.Ceiling((180 + decLongAbs) / 6))
            Else
                Return Convert.ToInt32(Math.Ceiling(decLongAbs / 6) + 30)
            End If

        End Function

        Public Function GetZoneCM() As Int32
            Return 6 * intZone - 183
        End Function

        Public Sub GetUTM()
            Dim a As Double = LookupA()
            Dim b As Double = LookupB()
            Dim f As Double = (a - b) / a
            Dim invf As Double = 1 / f
            Dim rm As Double = (a * b) ^ 0.5
            Dim e As Double = Math.Sqrt(1 - (b / a) ^ 2)
            Dim e1sq As Double = e ^ 2 / (1 - e ^ 2)
            Dim n As Double = (a - b) / (a + b)
            Dim latRad As Double = cooLat.GetDecimalCoordinate * Math.PI / 180
            Dim rho As Double = a * (1 - e ^ 2) / ((1 - (e * Math.Sin(latRad)) ^ 2) ^ (3 / 2))
            Dim nu As Double = a / ((1 - (e * Math.Sin(latRad)) ^ 2) ^ (1 / 2))

            Dim a0 As Double = a * (1 - n + (5 * n * n / 4) * (1 - n) + (81 * n ^ 4 / 64) * (1 - n))
            Dim b0 As Double = (3 * a * n / 2) * (1 - n - (7 * n * n / 8) * (1 - n) + 55 * n ^ 4 / 64)
            Dim c0 As Double = (15 * a * n * n / 16) * (1 - n + (3 * n * n / 4) * (1 - n))
            Dim d0 As Double = (35 * a * n ^ 3 / 48) * (1 - n + 11 * n * n / 16)
            Dim e0 As Double = (315 * a * n ^ 4 / 51) * (1 - n)
            Dim s As Double = a0 * latRad - b0 * Math.Sin(2 * latRad) + c0 * Math.Sin(4 * latRad) - d0 * Math.Sin(6 * latRad) + e0 * Math.Sin(8 * latRad)

            Dim p As Double = (cooLong.GetDecimalCoordinate - GetZoneCM()) * 3600 / 10000
            Dim sin1 As Double = Math.PI / (180 * 3600)

            Dim ki As Double = s * k0
            Dim kii As Double = nu * Math.Sin(latRad) * Math.Cos(latRad) * sin1 ^ 2 * k0 * (100000000) / 2
            Dim kiii As Double = ((sin1 ^ 4 * nu * Math.Sin(latRad) * Math.Cos(latRad) ^ 3) / 24) * (5 - Math.Tan(latRad) ^ 2 + 9 * e1sq * Math.Cos(latRad) ^ 2 + 4 * e1sq ^ 2 * Math.Cos(latRad) ^ 4) * k0 * (10000000000000000)
            Dim kiv As Double = nu * Math.Cos(latRad) * sin1 * k0 * 10000
            Dim kv As Double = (sin1 * Math.Cos(latRad)) ^ 3 * (nu / 6) * (1 - Math.Tan(latRad) ^ 2 + e1sq * Math.Cos(latRad) ^ 2) * k0 * (1000000000000)
            Dim a6 As Double = ((p * sin1) ^ 6 * nu * Math.Sin(latRad) * Math.Cos(latRad) ^ 5 / 720) * (61 - 58 * Math.Tan(latRad) ^ 2 + Math.Tan(latRad) ^ 4 + 270 * e1sq * Math.Cos(latRad) ^ 2 - 330 * e1sq * Math.Sin(latRad) ^ 2) * k0 * (1.0E+24)

            dblEasting = 500000 + (kiv * p + kv * p ^ 3)
            dblNorthing = (ki + kii * p * p + kiii * p ^ 4)
            If cooLat.Direction = CardinalDirection.South Then dblNorthing = 10000000 + dblNorthing
        End Sub

        Public Sub GetLatLong()
            Dim a As Double = LookupA()
            Dim b As Double = LookupB()
            Dim f As Double = (a - b) / a
            Dim invf As Double = 1 / f
            Dim rm As Double = (a * b) ^ 0.5
            Dim ec As Double = Math.Sqrt(1 - (b / a) ^ 2)
            Dim eisq As Double = ec ^ 2 / (1 - ec ^ 2)

            dblNorthing = If(utmL = UTMLat.North, dblNorthing, 10000000 - dblNorthing)

            Dim arc As Double = dblNorthing / k0
            Dim mu As Double = arc / (a * (1 - ec ^ 2 / 4 - 3 * ec ^ 4 / 64 - 5 * ec ^ 6 / 256))
            Dim ei As Double = (1 - (1 - ec * ec) ^ (1 / 2)) / (1 + (1 - ec * ec) ^ (1 / 2))
            Dim ca As Double = 3 * ei / 2 - 27 * ei ^ 3 / 32
            Dim cb As Double = 21 * ei ^ 2 / 16 - 55 * ei ^ 4 / 32
            Dim ccc As Double = 151 * ei ^ 3 / 96
            Dim cd As Double = 1097 * ei ^ 4 / 512
            Dim phi1 As Double = mu + ca * Math.Sin(2 * mu) + cb * Math.Sin(4 * mu) + ccc * Math.Sin(6 * mu) + cd * Math.Sin(8 * mu)

            Dim sin1 As Double = Math.PI / (180 * 3600)
            Dim q0 As Double = eisq * Math.Cos(phi1) ^ 2
            Dim t0 As Double = Math.Tan(phi1) ^ 2
            Dim n0 As Double = a / (1 - (ec * Math.Sin(phi1)) ^ 2) ^ (1 / 2)
            Dim r0 As Double = a * (1 - ec * ec) / (1 - (ec * Math.Sin(phi1)) ^ 2) ^ (3 / 2)
            Dim dd0 As Double = (500000 - dblEasting) / (n0 * k0)

            Dim fact1 As Double = n0 * Math.Tan(phi1) / r0
            Dim fact2 As Double = dd0 * dd0 / 2
            Dim fact3 As Double = (5 + 3 * t0 + 10 * q0 - 4 * q0 * q0 - 9 * eisq) * dd0 ^ 4 / 24
            Dim fact4 As Double = (61 + 90 * t0 + 298 * q0 + 45 * t0 * t0 - 252 * eisq - 3 * q0 * q0) * dd0 ^ 6 / 720

            Dim lof1 As Double = dd0
            Dim lof2 As Double = (1 + 2 * t0 + q0) * dd0 ^ 3 / 6
            Dim lof3 As Double = (5 - 2 * q0 + 28 * t0 - 3 * q0 ^ 2 + 8 * eisq + 24 * t0 ^ 2) * dd0 ^ 5 / 120

            Dim lat As Double = 180 * (phi1 - fact1 * (fact2 + fact3 + fact4)) / Math.PI
            If utmL = UTMLat.South Then lat = -lat
            cooLat = New Coordinate(Convert.ToDecimal(lat), CoordinateType.Latitude)

            Dim lon As Double = GetZoneCM() - ((lof1 - lof2 + lof3) / Math.Cos(phi1)) * 180 / Math.PI

            cooLong = New Coordinate(Convert.ToDecimal(lon), CoordinateType.Longitude)
        End Sub

        Private Function LookupA() As Double
            Select Case utmDat
                Case UTMDatum.NAD83, UTMDatum.WGS84, UTMDatum.GRS80
                    Return 6378137
                Case UTMDatum.WGS72
                    Return 6378135
                Case UTMDatum.Australian1965
                    Return 6378160
                Case UTMDatum.Krasovsky1940
                    Return 6378245
                Case UTMDatum.NorthAmerican1927
                    Return 6378206.4
                Case UTMDatum.International1924, UTMDatum.Hayford1909
                    Return 6378388
                Case UTMDatum.Clarke1880
                    Return 6378249.1
                Case UTMDatum.Clarke1866
                    Return 6378206.4
                Case UTMDatum.Bessel1841
                    Return 6377397.2
                Case UTMDatum.Airy1830
                    Return 6377563.4
                Case UTMDatum.Everest1830
                    Return 6377276.3
            End Select
        End Function

        Private Function LookupB() As Double
            Select Case utmDat
                Case UTMDatum.NAD83, UTMDatum.WGS84
                    Return 6356752.3142
                Case UTMDatum.GRS80
                    Return 6356752.3141
                Case UTMDatum.WGS72
                    Return 6356750.5
                Case UTMDatum.Australian1965
                    Return 6356774.7
                Case UTMDatum.Krasovsky1940
                    Return 6356863
                Case UTMDatum.NorthAmerican1927
                    Return 6356583.8
                Case UTMDatum.International1924, UTMDatum.Hayford1909
                    Return 6356911.9
                Case UTMDatum.Clarke1880
                    Return 6356514.9
                Case UTMDatum.Clarke1866
                    Return 6356583.8
                Case UTMDatum.Bessel1841
                    Return 6356079
                Case UTMDatum.Airy1830
                    Return 6356256.9
                Case UTMDatum.Everest1830
                    Return 6356075.4
            End Select
        End Function

        Public Function GetZoneChar() As Char
            Select Case cooLat.GetDecimalCoordinate
                Case -90 To -84
                    Return "A"c
                Case -84 To -72
                    Return "C"c
                Case -72 To -64
                    Return "D"c
                Case -64 To -56
                    Return "E"c
                Case -56 To -48
                    Return "F"c
                Case -48 To -40
                    Return "G"c
                Case -40 To -32
                    Return "H"c
                Case -32 To -24
                    Return "J"c
                Case -24 To -16
                    Return "K"c
                Case -16 To -8
                    Return "L"c
                Case -8 To 0
                    Return "M"c
                Case 0 To 8
                    Return "N"c
                Case 8 To 16
                    Return "P"c
                Case 16 To 24
                    Return "Q"c
                Case 24 To 32
                    Return "R"c
                Case 32 To 40
                    Return "S"c
                Case 40 To 48
                    Return "T"c
                Case 48 To 56
                    Return "U"c
                Case 56 To 64
                    Return "V"c
                Case 64 To 72
                    Return "W"c
                Case 72 To 84
                    Return "X"c
                Case Else
                    Return "Z"c
            End Select
        End Function

    End Class
#End Region

End Class

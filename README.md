# README #

Simple project demonstrating how the Z value reported by aXiom can be used in a Windows application.

Requires the bridge (TNxPB005, TNxPB007, AXPB009 or AXPB011) to be in digitizer mode.

## Z pressure ranges ##
The pressure field in the Windows digitizer class reports in the range 0 -> 1024; aXiom reports Z in the range -128 -> 127 (proximity, hover and force). The table below shows how the ranges reported in Windows correlate to the Z value reported by aXiom:

| State     | aXiom      | Windows    |
| --------- | ---------- | ---------- |
| Proximity | -128       | 128        |
| Hover     | -127 -> -1 | 129 -> 255 |
| Touch     | 0          | 0          |
| Force     | 1 -> 127   | 1 -> 127   |

To convert from the pressure reported by Windows back to the aXiom Z range, perform the following:

1. Divide by 4 (Right shift 2)
2. Subtract 1
3. Cast to a signed 8-bit value
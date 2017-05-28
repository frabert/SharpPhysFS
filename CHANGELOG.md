## Version 0.0.1
- Initial release

## Version 0.0.2
- Added cdecl calling convention for StringCallback

## Version 0.1.0
- *THIS VERSION INCLUDES MAJOR BREAKING CHANGES*
- The library is no longer static

## Version 0.2.0
- New dataless enumeration functions are available now, which do not cause GC havoc
- Unsafe enumeration functions are now marked obsolete

## Version 1.0.0
- DLL configuration on platforms other than Windows is done via app.config now, as per http://www.mono-project.com/docs/advanced/pinvoke/dllmap/
- Fixed a bug in which trying to open a nonexistant bug would create an invalid handle without throwing exceptions
- Fixed a bug which would ignore the offset parameter in stream readings
- Added automated testing
- Various deprecated methods are now private only, they have been substituted with safer counterparts
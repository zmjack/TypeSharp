call dotnet ts tsg -o scripts/typings/@auto -n ^
-i "Ajax.JSend`1,JSend" ^
-r "Ajax.JSend,JSend;Ajax.JSend<any>"

call dotnet ts tsapi -o scripts/components ^
-r "Ajax.JSend,JSend;Ajax.JSend<any>"

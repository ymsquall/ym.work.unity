function PrintTable(t,n,saved)
	saved = saved or {}
	n = n or 0
	for k in pairs(t) do
		local str = ''
		if n~=0 then
			local fmt = '%' .. 2*n .. 's'
			str = string.format(fmt, '')
		end
		--print(str,tostring(k), ' = ')
		io.write(str,tostring(k), ' = ')
		if type(t[k])=='table' then
			local m = n
			m = m+1
			if saved[t[k]] then
				--print(saved[t[k]], '\n')
				io.write(saved[t[k]], '\n')
			else
				saved[t[k]] = k
				--print('{\n')
				io.write('{\n')
				PrintTable(t[k], m, saved)
				--print(str,'}\n')
				io.write(str,'}\n')
			end
	   else
			--print(tostring(t[k]),'\n')
			io.write(tostring(t[k]),'\n')
	   end
	end
end

function stringSplit(str, mark)
    local sub_str_tab = {};
    while (true) do
        local pos = string.find(str, mark);
        if (not pos) then
            sub_str_tab[#sub_str_tab + 1] = str;
            break;
        end
        local sub_str = string.sub(str, 1, pos - 1);
        sub_str_tab[#sub_str_tab + 1] = sub_str;
        str = string.sub(str, pos + 1, #str);
    end
    return sub_str_tab;
end

dofile 'D:\\work\\ym.work.unity\\project\\0001.struggle_of_fight\\Assets\\LuaScript\\luautility.lua'

__LUASceneVariable =
{
	name = 'scene',
	grids = {},

}

function LUAInner_ParseScrollGroup(sindex, tbl)
	local sc = _Map2DGrids.scroll[sindex]
	sc.c = sc.c or 1
	for c = 1,sc.c,1 do

	end
	return tbl
end

function LUACSCall_SetSceneVar(name)
	print('LUACSCall_SetSceneVar = '..name)
	__LUASceneVariable.name = name
	--local pMap2DGrids = require ('LuaScript.Scenes.'..name..'.grids')
	local pMap2DGrids = require ('.grids')
	local final = false
	while(not final) do
		local rowNum = 0
		local colNum = 0
		local firstRow = pMap2DGrids.grid[1]
		for _,v in pairs(firstRow) do
			if type(v) == 'table' and v[1] ~= nil then
				local scroll = pMap2DGrids.scroll[v[1]]
				if scroll.t == 'col' or scroll.t == 'all' then
					local group = pMap2DGrids.group[scroll.g[1]]
					colNum = colNum + #group * scroll.c[1]
				else
					error('grid scroll format error!!!first row must be col or all scroll group!!!!')
				end
			else
				colNum = colNum + 1
			end
		end
		for _,row in pairs(pMap2DGrids.grid) do
			--PrintTable(row)
			local firstCol = row[1]
			if type(firstCol) == 'table' and firstCol[1] ~= nil then
				local scroll = pMap2DGrids.scroll[firstCol[1]]
				if scroll.t == 'row' then
					local group = pMap2DGrids.group[scroll.g[1]]
					rowNum = rowNum + #group * scroll.c[1]
				elseif scroll.t == 'all' then
					local group2 = pMap2DGrids.group[scroll.g2[1]]
					rowNum = rowNum + #group2 * scroll.c2[1]
				else
					error('grid scroll format error!!!first col must be row or all scroll group!!!!')
				end
			else
				rowNum = rowNum + 1
			end
		end
		print('row='..rowNum..', col='..colNum)
		local rowIndex = 1
		local colIndex = 1
		for _,row in pairs(pMap2DGrids.grid) do
			local newRow = {}
			table.insert(__LUASceneVariable.grids, rowIndex, newRow)
			for _,col in pairs(row) do
				if type(col) == 'table' and col[1] ~= nil then
					local scroll = pMap2DGrids.scroll[col[1]]
					if scroll.t == 'row' then
						for _
					elseif scroll.t == 'col' then
					elseif scroll.t == 'all' then
						error('grid scroll format error!!!first row must be col or all scroll group!!!!')
					end
				else
					table.insert(newRow, colIndex, col)
				end
				colIndex = colIndex + 1
			end
			rowIndex = rowIndex + 1
			colIndex = 1
		end
		PrintTable(__LUASceneVariable.grids)
	end
end

LUACSCall_SetSceneVar('')

return
{
    LUASetSceneVar = LUASetSceneVar,
}

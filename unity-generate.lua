
template = [[
			public void RegisterCallback($WrappedCallback callback) {
				Start(); // make sure the interface is initialized.
				if (null == $WrappedList) {
					$WrappedList = new List<$WrappedCallback>();
					iface.registerCallback ($RawCallback, System.IntPtr.Zero);
				}
				$WrappedList.Add (callback);
			}
			
			private List<$WrappedCallback> $WrappedList;
]]
function generate(data)
	s = string.gsub(template, "$(%w+)", function (n)
		return data[n]
	end)
	return s
end

print "/* BEGIN GENERATED CODE - unity-generate.lua */"
print(generate{
	WrappedCallback = "PoseMatrixCallback";
	WrappedList = "poseMatrixCallbacks";
	RawCallback = "PoseMatrixCb";
})

for _, v in ipairs{"Pose", "Position", "Orientation", "Button", "Analog"} do
	local CapsName = v
	local nocapsName = v:sub(1, 1):lower() .. v:sub(2)
	local vals = {
		WrappedCallback = CapsName .. "Callback";
		WrappedList = nocapsName .. "Callbacks";
		RawCallback = CapsName .. "Cb";
	}
	print(generate(vals))
end

print "/* END GENERATED CODE - unity-generate.lua */"
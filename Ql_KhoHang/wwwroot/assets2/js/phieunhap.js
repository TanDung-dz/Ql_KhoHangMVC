

	$(function () {
		$(".selectdetail").select2({
			placeholder: '-- Chọn quy cách đá --',
			matcher: matchCustom,
			templateResult: formatOption,
		}).on('select2:opening', function (e) {
			$(this).data('select2').$dropdown.find(':input.select2-search__field').attr('placeholder', 'Tìm kiếm ...')
		});
		function stringMatch(term, candidate) {
			return candidate && candidate.toLowerCase().indexOf(term.toLowerCase()) >= 0;
		}

		function matchCustom(params, data) {
			// If there are no search terms, return all of the data
			if ($.trim(params.term) === '') {
				return data;
			}
			// Do not display the item if there is no 'text' property
			if (typeof data.text === 'undefined') {
				return null;
			}
			// Match text of option
			if (stringMatch(params.term, data.text)) {
				return data;
			}
			// Match attribute "data-foo" of option
			if (stringMatch(params.term, $(data.element).attr('title'))) {
				return data;
			}
			// Return `null` if the term should not be displayed
			return null;
		}

		function formatOption(option) {
			var $option = $(
				'<div>' + option.text + '</div><div>' + option.title + '</div>'
				);
				return $option;
			};
	});


function AddItem(btn) {

	var table;
	table = document.getElementById('CodesTable');
	var rows = table.getElementsByTagName('tr');
	var rowOuterHtml = rows[rows.length - 1].outerHTML;

	var lastrowIdx = rows.length - 2;

	var nextrowIdx = eval(lastrowIdx) + 1;

	rowOuterHtml = rowOuterHtml.replaceAll('_' + lastrowIdx + '_', '_' + nextrowIdx + '_');
	rowOuterHtml = rowOuterHtml.replaceAll('[' + lastrowIdx + ']', '[' + nextrowIdx + ']');
	rowOuterHtml = rowOuterHtml.replaceAll('-' + lastrowIdx, '-' + nextrowIdx);

	var newRow = table.insertRow();
	newRow.innerHTML = rowOuterHtml;

	// select2 custom *begin*
	$(function () {
		$(".selectdetail").select2({
			placeholder: '-- Chọn quy cách đá --',
			matcher: matchCustom,
			templateResult: formatOption
		}).on('select2:opening', function (e) {
			$(this).data('select2').$dropdown.find(':input.select2-search__field').attr('placeholder', 'Tìm kiếm ...')
		});
		function stringMatch(term, candidate) {
			return candidate && candidate.toLowerCase().indexOf(term.toLowerCase()) >= 0;
		}

		function matchCustom(params, data) {

			if ($.trim(params.term) === '') {
				return data;
			}
			if (typeof data.text === 'undefined') {
				return null;
			}
			if (stringMatch(params.term, data.text)) {
				return data;
			}
			if (stringMatch(params.term, $(data.element).attr('title'))) {
				return data;
			}
			return null;
		}

		function formatOption(option) {
			var $option = $(
				'<div>' + option.text + '</div><div>' + option.title + '</div>'
			);
			return $option;
		};

		$(".selectdetail").last().next().next().remove();
	});


	// select2 custom *end*
	var sloaithungInput = document.getElementById('sloaithung-' + nextrowIdx);
	var isothungInput = document.getElementById('isothung-' + nextrowIdx);
	var idongiaInput = document.getElementById('idongia-' + nextrowIdx);
	var ivatInput = document.getElementById('ivat-' + nextrowIdx);

	var isdongiaDisabled = idongiaInput.disabled;



	/*var isDisabled = idientichdatInput.disabled;*/

	// Khởi chạy lại khi tạo mới
	isothungInput.oninput = function () {
		ThanhTien(nextrowIdx);
		formatCurrency(isothungInput);
	};

	idongiaInput.oninput = function () {
		ThanhTien(nextrowIdx);
		formatCurrency(idongiaInput);
		Kiemtradongia(idongiaInput);
	};

	ivatInput.oninput = function () {
		TienThue(nextrowIdx);
		validateInput(ivatInput);
	};


	if (!isdongiaDisabled) {
		isothungInput.disabled = true;
		idongiaInput.disabled = true;
		ivatInput.disabled = true;
	};

	sloaithungInput.value = '';
	isothungInput.value = '';
	ivatInput.value = '';
	idongiaInput.value = '';
	$(idongiaInput).removeClass('is-valid is-invalid'); 

	var sttColumn = newRow.querySelector('td[id="stt"]');

	if (sttColumn) {

		var currentValue = parseInt(sttColumn.textContent, 10);
		var newValue = currentValue + 1;
		sttColumn.textContent = newValue;
	}

	Capnhatstt();
	Tongtienhang();
	Tongthue();
	Tongtien();


}

	function DeleteItem(btn) {

		var table = document.getElementById('CodesTable');
		var rows = table.getElementsByTagName('tr');

		var visibleRowCount = 0;
		for (var i = 1; i < rows.length; i++) {
			if (rows[i].style.display !== 'none') {
				visibleRowCount++;
			}
		}

		if (visibleRowCount == 1) {
			return;
		}
		else {
			$(btn).closest('tr').hide();

		}



	for (var i = 1; i < rows.length; i++) { // Bắt đầu từ 1 để tránh hàng tiêu đề
			var sttColumn = rows[i].querySelector('td[id="stt"]');
	var icn = i - 1;
	if (sttColumn) {
		// Cập nhật giá trị của cột td để bắt đầu từ 0 và tăng dần
		sttColumn.textContent = i;
			}


		}

	// var btnIdx = btn.id.replaceAll('btnremove-', '');
	// var idOfQuantity = btnIdx + "__Quantity";
	// var txtQuantity = document.querySelector("[id$='" + idOfQuantity + "']");

	// txtQuantity.value = 0;


	// var idOfIsDeleted = btnIdx + "__IsDeleted";
	// var txtIsDeleted = document.querySelector("[id$='" + idOfIsDeleted + "']");
	// txtIsDeleted.value = "true";

	Capnhatstt();
	Tongtienhang();
	Tongthue();
	Tongtien();

	}


function Capnhatstt() {
	var table = document.getElementById('CodesTable');
	var rows = table.getElementsByTagName('tr');
	var stt = 1;

	for (var i = 1; i < rows.length; i++) { // Bắt đầu từ 1 để tránh hàng tiêu đề
		var rowData = rows[i];
		if (rowData.style.display !== 'none') {
			var sttColumn = rowData.querySelector('td[id="stt"]');
			if (sttColumn) {
				sttColumn.textContent = stt;
				stt++;
			}
		}
	}
}




	function Tongtienhang() {
		var total = 0;

		var ithanhtienElements = document.querySelectorAll('[id^="ithanhtien-"]');

		ithanhtienElements.forEach(function (element) {
		var elementValue = element.value;
		var elementValueWithoutCommas = elementValue.replace(/,/g, '');

		var ithanhtien = parseFloat(elementValueWithoutCommas) || 0;
		var parentRow = element.closest('tr');
		if (parentRow.style.display !== 'none') {
			total += ithanhtien;
		}	
	});


	var itongtienhang = document.getElementById("itongtienhang");
		if (itongtienhang) {
			var formatter = new Intl.NumberFormat('vi-VN');
			var formattedTotal = formatter.format(total);
			formattedTotal = formattedTotal.replace(/\./g, ',');
			itongtienhang.value = formattedTotal;
		}
	}

	function TienThue(row){
		var isothungStr = document.getElementById('isothung-' + row).value;
		var isothungWithoutCommas = isothungStr.replace(/,/g, '');
		var isothung = parseFloat(isothungWithoutCommas) || 0;

		var idongia = document.getElementById('idongia-' + row).value;
		var idongiaWithoutCommas = idongia.replace(/,/g, '');
		var idongia = parseFloat(idongiaWithoutCommas) || 0;

		var ivat = document.getElementById('ivat-' + row).value;
		var ivatWithoutCommas = ivat.replace(/,/g, '');
		var ivat = parseFloat(ivatWithoutCommas) || 0;

		var ithanhtien = isothung * idongia;
		var ivatthanhtien = ithanhtien * ivat / 100;

		var formatter = new Intl.NumberFormat('vi-VN');
		var formattedValue = formatter.format(ivatthanhtien);
		formattedValue = formattedValue.replace(/\./g, ',');
		document.getElementById('ivatthanhtien-' + row).value = formattedValue;
		Tongtienhang();
		Tongthue();
		Tongtien();
	}

	function Tongthue() {
		var itongthue = 0;

		var ivatthanhtienElements = document.querySelectorAll('[id^="ivatthanhtien-"]');

		ivatthanhtienElements.forEach(function (element) {
			var elementValue = element.value;
			var elementValueWithoutCommas = elementValue.replace(/,/g, '');
			var ivatthanhtien = parseFloat(elementValueWithoutCommas) || 0;
			var parentRow = element.closest('tr');
			if (parentRow.style.display !== 'none') {
				itongthue += ivatthanhtien;
			}	

		});

		var itongthueElement = document.getElementById("itongthue");
		if (itongthueElement) {
			var formatter = new Intl.NumberFormat('vi-VN');
			var formattedTotal = formatter.format(itongthue);
			formattedTotal = formattedTotal.replace(/\./g, ',');
			itongthueElement.value = formattedTotal;
		}

		Tongtien();
	}

	function Tongtien() {
		var total = 0;


		var itongtienhangStr = document.getElementById("itongtienhang").value;
		var itongtienhangWithoutCommas = itongtienhangStr.replace(/,/g, '');
		var itongtienhang = parseFloat(itongtienhangWithoutCommas) || 0;


		var itongthueStr = document.getElementById("itongthue").value;
		var itongthueWithoutCommas = itongthueStr.replace(/,/g, '');
		var itongthue = parseFloat(itongthueWithoutCommas) || 0;


		total = itongtienhang + itongthue;

		var itongtien = document.getElementById("itongtien");
		if (itongtien) {
			var formatter = new Intl.NumberFormat('vi-VN');
			var formattedTotal = formatter.format(total);
			formattedTotal = formattedTotal.replace(/\./g, ',');
			itongtien.value = formattedTotal;
		}
	}

	window.onload = Tongtienhang;
	window.onload = Tongthue;
	window.onload = Tongtien;

function ThanhTien(row) {

		var isothungStr = document.getElementById('isothung-' + row).value;
		var isothungWithoutCommas = isothungStr.replace(/,/g, '');
		var isothung = parseFloat(isothungWithoutCommas) || 0;

		var idongia = document.getElementById('idongia-' + row).value;
		var idongiaWithoutCommas = idongia.replace(/,/g, '');
		var idongia = parseFloat(idongiaWithoutCommas) || 0;


		var ithanhtien = isothung * idongia;
		var formatter = new Intl.NumberFormat('vi-VN');
		var formattedTotal = formatter.format(ithanhtien);
		formattedTotal = formattedTotal.replace(/\./g, ',');
		document.getElementById('ithanhtien-' + row).value = formattedTotal;

	// Gọi lại hàm Tongtienhang khi dữ liệu thay đổi
		TienThue(row);
		Tongtienhang();
		Tongthue();
		Tongtien();

	}

	function Xoadonghide() {
		var table = document.getElementById('CodesTable');
		var rows = table.getElementsByTagName('tr');
		var ip = 0;
	
		for (var i = 1; i < rows.length; i++) { // Bắt đầu từ 1 để tránh hàng tiêu đề
			ip = i - 1;
			var row = rows[i];
			var isHidden = row.style.display === 'none';
			var xacnhanInput = document.getElementById('ixacnhan-' + ip);

			if (xacnhanInput && isHidden) {
				xacnhanInput.value = 'false';
			}

		}
	}



	//document.addEventListener("DOMContentLoaded", function () {
	//	var rowCount = 1;
	//var rows = document.querySelectorAll("table tbody tr");
	//rows.forEach(function (row) {
	//	row.querySelector("#sttbang").textContent = rowCount;
	//rowCount++;
	//	});
	//});



	function formatCurrency(input)
	{
		var value = input.value.replace(/\D/g, '');
		var formatter = new Intl.NumberFormat('en-US');
		var formattedValue = formatter.format(value);

		/*formattedValue = formattedValue.replace(/\./g, ',');*/

		if (value < 0)
		{
			alert("Giá nhập không hợp lệ. Vui lòng nhập lại!");
		input.value = "0";
		}
		else
		{
			input.value = formattedValue;
		}
	}

	function validateInput(input) {
		var value = input.value;
		value = value.replace(/[^0-9]/g, '');
		var intValue = parseInt(value, 10);

		if (isNaN(intValue)) {
			intValue = 0; 
		} else if (intValue < 1) {
			intValue = 1;
		} else if (intValue > 15) {
			intValue = 15;
		}

		input.value = intValue;
	}

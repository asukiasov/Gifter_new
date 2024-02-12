$(function(){
	//--- virtual select
	VirtualSelect.init({
		ele: '.js-combo',
		hideClearButton: true,
		optionHeight: '30px'
	});

	VirtualSelect.init({
		ele: '.js-combo-multiple',
	});


	//--- modals
	$('.js-modal-show-btn').click(function (){
		$('.js-modal').modal({
			show: true,
			backdrop: 'static'
		});
	});

	$('.js-modal-lg-show-btn').click(function (){
		$('.js-modal-lg').modal({
			show: true,
			backdrop: 'static'
		});
	});

	$('.js-modal-xl-show-btn').click(function (){
		$('.js-modal-xl').modal({
			show: true,
			backdrop: 'static'
		});
	});

	$('.js-msg-modal-show-btn').click(function (){
		$('.js-msg-modal').modal({
			show: true
		});
	});
});
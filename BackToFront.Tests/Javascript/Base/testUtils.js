(function (global) {

    var tUtil = {};

    tUtil.Expect = function () {

        this.Expectations = [];
        this.ExpectationReached = [];

        for (var i = 0, ii = arguments.length; i < ii; i++) {
            this.Expectations.push(arguments[i]);
        }
    };

    tUtil.Expect.prototype.VerifyOrderedExpectations = function () {

        for (var i = 0, ii = this.Expectations.length; i < ii; i++) {
            if (this.Expectations[i] !== this.ExpectationReached[i]) {
                assert.ok(false, "Expectatation \"" + this.Expectations[i] + "\" was not reached.");
                return;
            }
        }

        assert.ok(this.Expectations.length >= this.ExpectationReached.length, this.ExpectationReached.length + " expectatations were reached, however only " + this.Expectations.length + " were defined.");
    };

    global.tUtil = tUtil;
})(window);
﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "86.0b6";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/86.0b6/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "e4eda9c24d87c9cf8fecec681e9647bf9a32f6127945a529c7db8d7d49d719086f38cfd91ad122848b53acebd7d1ca77bb97fc06f83bcc702ebdca709fdd04c3" },
                { "af", "3017bed1618001ac89339e08a980614a3e29afc5f9ebc561f880fbdebeb2d544f958bb21b8e39a2286d9ddc36d648fe3ef8fc4e64d10d583543556b04d08909f" },
                { "an", "d15567971c3bca1ee76d4104048ac7c2ead6917225f82a99dddfb0cde3b54a60fd31dc880255ea19efa66ab8f6d399bc7bfe2401c97da6ba8c6be342f49dbc4c" },
                { "ar", "bcb62fdae4ec4fa4f6fab9770b3a6cdab6e44b39dbf3ff22e533e1404c2535447013d0899a4dea418655bcf3814ccd9ccb177c6f093a308c2eb6e14adafc5ff3" },
                { "ast", "be652974835b96c1f24ab980b9c231ffa95ac66efe5792b4293b74db0d24222ade2d79d3a6eb2bbe896e796107c04f367f4d6b59a7e4797712eb636104b143c3" },
                { "az", "adfac352875a3aaa049b0c3a7ec229d3ca188923c190db7b7917c16bbcf93486393e0b00d7c850bc698432f38800bd3106c0e27054f264e1efb7e7c5e2b71f6a" },
                { "be", "34548ee5fdf1f1301f60bc40776123dd2ffd063ec5967a14417af3d0cb754e1a7441a9567026c1e9108d98adf2cc90d4c17664055f4a796515eccb5cf60ea3a6" },
                { "bg", "29ed96e9de67a3fea1884453240d2715da2b770daa0c5f6762b52cefe76a9d8ea8924db66abfc561b35b04fef02985b0b4b7fe96eade4f77fc3fc2cdf8b07802" },
                { "bn", "65be099d96dec33ed5283f99c64c8730a584ccfbcec34cab0c5d0d21b10de315ea166ab9c5f817c0084ab57e00f81f823ca6355a67c0ed36162434f5ebf7f652" },
                { "br", "9112927e0e9406a8553f031f63053bed30390eb30c15ac85891f708d997875574f1258b82a2f25f6d400c346167f5dc94a4fc48aa906a78f313b5ff445d6f918" },
                { "bs", "6ff25b60cce6d7b476f1f1d3117a40c98e14e974fbe8f84197f5d5ca4cc2552c05068be9409b37cf9df657cb48f084282e4016d29810e1f08dff26d5d0f09414" },
                { "ca", "9c7c2228be84ad373a9d6cbc1abf06ed4958064230af6b8bcceea62a9060ba0fa65ab38f876780d85ac8b20ca08b5b729a45c33a2fd9078226751b67977e374a" },
                { "cak", "673228eb668c1eb7c71a9ae49545342b713866793864e79522196832e6314f4352a4ddcf1b0e93f4576588a4719a56f0d28b888d29a5253ab129dbe0a6b6eaa8" },
                { "cs", "64aeaad4177eeedbc363b0acc49ec0320b73461857103843c3822cfe61455fa54fbdf73451ebded349bb59d9c713341fdf5b765358764749e42f4706b9ee941c" },
                { "cy", "2d319e5a11e676860351e57f874f260c843e19033afa977df051a4ea48c90aeee3ce3774b0319419546426e5dce9bc3ac6cf770f8abf02ecdac836d4856c9c4f" },
                { "da", "a8cec678d25697eb2951a497d87111c4cb56d3150bfab66c3b5982dac861b35361919c81c5ace215fcafe5bce51d7de8e029a33a829a8650a2919a8bc4577ec4" },
                { "de", "c580a0cb43c22bbf38cbaed065e411aa71078b9539a49760a8f31be12c018d18c401ce809c7786db04d8704d2537c7199c830ba7533f6f99672cde9c212288e7" },
                { "dsb", "1ac4c53fe5a6c4089e9e318bd694dfc75b0d5dbecd73425180d33e843c241bc1fe1ddf696cac07ebb2cac52042e57f778cfb5a8b08df6544e6d8064fcaf95276" },
                { "el", "6b6b6b1d6f8e2e5843669ae99d4a885258de5d4ab432ce617b11c1ba57614117b391aaf77e77b277dc732a52103307653cbd8e84c7d74c91ef8f018c3c09ce22" },
                { "en-CA", "e0c0c2bf089beb8befc6d3f2dd2193b7d082f3fd8a223cab821ba27e3c75965d41a5202717c956202badcd0be0711db5cd618768e1d9d3c80f0bb36d38ffec51" },
                { "en-GB", "b1616057d785e198226a3665ea659048631c650a06738a4f17c43bf72a461045070c0d1910020d856e281e379c8bb7b51b301f8ee6ea517d1673ec0175ca8d04" },
                { "en-US", "7e5876ac53382577230f80e9a64893c28e3890a8b62a02a6fd5b7c4acf936221bd209fb40f1a4c4f831b612a4edb3597cdf3272310ad4d57f6b5c85a7e1c5f7a" },
                { "eo", "8413337aaaf2b907045153ab1eac0792a90f37751397b2317bfedeb137aced12495e2ec468ac8e02d4053ce411fbd817cc7ce44abf73dea03bb0d6656e2183b8" },
                { "es-AR", "67f1ce36b51738e5a6de3f30b11afef012ac6ac8129341f8b53fe126b0b49b356266f81266cc1849d1e53d073255a6117b5c8c034e815b0d6cb31f5b6944bcf0" },
                { "es-CL", "fa3beae0ef9dce563868d8f79e4325a785161b5a1738139a8656ba51f83da264168c6d750902bc8c76da43bd8c2ab84a4fdc7332e63d6eec8b245a50e89a8f88" },
                { "es-ES", "7b0f00a1a99cc5e1a7d7e4b5eaa1e9977e76a78fc9598f7bb43f1effa38e315573e92ead8d2fd09195d816686a41e208448f5ae5ff65ce9501333b05084e0229" },
                { "es-MX", "55169b46d306584ab29e5f25a8c2840cb38fa6d07fd930eb6b68cec8ef87dee8d67809922a324b9dd5a0d37140c6a5188c5edb48f109ca4930aa49499271e7af" },
                { "et", "85f21834b0ac20a6895f8c4845c584673d525d2887a0b49a3fccd2d77f6380f52d61abf025d29936a5d45b908073e601816483916622dd6efbc05302fe8337e7" },
                { "eu", "7a7902f14d40414ee5207643f0fcb503d01bc187c1427628d17560f2907244c8fc22d81858a1227199e9980073f019aa956f50158ed341f5b50fded209af643a" },
                { "fa", "de90e6de669ede1e9d903b6beb8fac60cc262f5acc90499fbf5f15b4c69650b946b0a79add87f685ba9a75bd0c1453bdedf409eeea179edca13f5fa13aa2bb1d" },
                { "ff", "1f5635cc9fb78d3bf80d0efb946fe4847189d7ea274b6002b40be14d97f5074df60a6a1f6f4b98fa01adf3d0463a3dd59bf08b316a5cfea721f70d3278da7779" },
                { "fi", "bb0e60f8917b3b34d61d599bfb22175ada12a9de260b184e44170d5dbcc6a04ec8c63014d2ed38cbd2cfb40fefa84fc42642446833ebfc954592f9c76f46d9ad" },
                { "fr", "dd633f89d9d05543f219ad7c7873ee4a0bdd4bbebf236671e2aebdafd440c6b99d09ce877da7b525b48dd161e090051d901ba25843c44ab9e77fec6f932af03f" },
                { "fy-NL", "4f4cd369debc5b2efe9873326bb4daa0134db90a76aee66fe02714d2b53c985f197677477e08c51cc4f7d86c83c1120e3182713b6dccde8ac3767c34e9d56c20" },
                { "ga-IE", "06d76abfa85112541d6cb8205774836b1897be21c281ba77935a0aee3964afaf86fb76e67f238d3b18e947bd926a96bd4915204937dc3aafde344a3cab94a4ef" },
                { "gd", "5c1544895ae3afa072c931c380251a2e8fcbbdf03dcf817fdd2296daa5aa65e48f486d58117dc4891055893910769a604950e801fcbdff48bc03e078e67ea890" },
                { "gl", "e74d64102e9c02127c0ee49f0a8d28f2974f6b8733e0080346e8b067c73ba8b99cf521d5af9f06f00c52fda89bb075b6a36c5c9763c971908656252dcc4b8fca" },
                { "gn", "be1ed0955724f37fad5fb3d82057074797cd7fe8b8909b875798fbdf4795c1b6fe376c1b3b71348f6895409a6d4a7d7603ecd90580407f1a7308e23b5ad753b9" },
                { "gu-IN", "ec111a50537708b5901dd01b70150a9e6f4c3360156296f1c370c619acf21623c4897c5ef97750a30b03fcced20e46ff1fedeafaa59b2b342df204e44e9946d9" },
                { "he", "e65550acd9cef782c52c6ca19f23e44d7368f73f7a9f03c99377182ebe8182c14ab30607376fdd6796682508c679c278014072801cbebebef9a0860f691b9a35" },
                { "hi-IN", "95711add262de1eb89733a2ebe2d8a3a69802b6b5a336ec4904ef01bf53be815968680e150008e3b730f6d472612970a1ab3f6c98e7e6dab765deb6142a25143" },
                { "hr", "d79ea1caab04b65705c09560ec141c0776511a3ffa73de3347528fd337c49861cb657f73c25adc683eebfa16de9bd9b3b2ccdbeacae9b77ba7f426d174e99fe2" },
                { "hsb", "b7bfad2763a03063fb9c3a6f8d9be035ad1e9fdfe1941fe5437e24851795a34f338fb75bdc0c0c9cf4358a238378f080c1d6739700935ac1d3427f364a30a6b0" },
                { "hu", "e458b11221cdb5989592217e03b91907b9bbcb489cdd91b8a7950d923d0edc96aee1db9cdd741e416d4b3f99c303356cc6cfe6edc9f751398af0e8f2daa1bf85" },
                { "hy-AM", "bc6c8ac8ad9429293d94f2de49cde6e1d19b44a517e3792f8c17baf3b4f4a10f20ee372c8edb09d694dd3f71886d43dc1284e8dc84b711db0ba8b1705098e14a" },
                { "ia", "0edd60bc376e5c5803a5815a36b16b63210304fce8527e671ba5eaa20631aa5d1edd013e6fd02bfcde4434f1d69815be68ce718fcbe27a7cbc0c747760c6ae36" },
                { "id", "ec6210b5aca41cc5034dc98f83542e4cc3b166e289fbec04cb78eef9478099710ad9353d30b91ce67c578943e46f190a4726a412ffd99902db95fc2982b3747d" },
                { "is", "17bc5704d9d80886e81423a59a5b9429fb38bd52bebff3a9e3363a402eabb30ab90c2d19f3b9788d8569f9341e095f4829da0615e26cfc3d7f1f88017d883c08" },
                { "it", "c6c3e63fdd6103f9ef406c4cd5bc592025e3b0fba1569a0ff16ad8c4eec50126d856eadd5c97ef135ece7593a1647c57e452a409acfa7abef04400cd9dc940b2" },
                { "ja", "0508fa63cfdb12aff41ee93d132a327800b58a9307b6e3575092bf2329066053d266478e24759af0042530e09d4f50fe6b678feb1e5d769dd2941ff4dc2c1bd4" },
                { "ka", "a17a8e133006b77add6c48b2310122c73131b4f6efb3ecc55d38e19b0fe0187c229bdbf53359eee71ad213e9e932158f178a7d60bc72660ee3d249bf8d8cb1f2" },
                { "kab", "d33a46ce1ef750244f231957aa48d5d919d5e31cc0d3af1be054b539615aad58a7026e3e6a2d8d6819cbc608892db775085f08b271f6124006556d023e258cdb" },
                { "kk", "059fb719c3a0bd8a2f8b95856042dfd224c48877cb7b15e12fa4f2983c14291f02cb014c8ba71cb376bcd99f087d55afc528bc64c75bbd747e7f089c558fb8c0" },
                { "km", "82cbe0da29b7d19f262a8a1ec0d7a64c8b62e49b3320c111d24b163a79dcc4424537e9402048d8b9111b40f34ffb8387d54b18e0fca93dd0f723b1f3b4958c81" },
                { "kn", "08e184c2db7a0179d53f0fbfaa4dbc48fd5c6b65331997893890aeba0324c0150a81ed66d94c2e0968f7fbc92736a63c62ab7696001d5112a150029d6395a277" },
                { "ko", "95b0949f937e64d04d3349633fd4585226886c61bf936a39140926bc2b16e96cb98bfa29e0f008335f9f80aae996c4059d38be584e6e68188df3f784425c74bd" },
                { "lij", "7fa5be4248e92c577e59bdd78125e2fc96606ae3410a77fd772d30485c75540e41502d7449b48a68f31dbc4a5dc4bbe794b516f518d471f54f34361b15a46298" },
                { "lt", "bd79f7fd5350da937474b87999706da2863b72746cd1e8a72e98a6cd1850d42e2b9495728bf03fa14daf676f9e7a8384f3c6b0dda1f57ae3d0e6b75289294e80" },
                { "lv", "f4ec2f8c50c2096cbee15a9a8e5d3fc7a7ca3de6b0274caf80d31d386364fe517a113ebf44f11edaf4242046f1970d968e21d5a380eb09803bda466924f1a252" },
                { "mk", "aae7ee33fec724c032db73450b82ab8040d0625318fecc5ee93e03867aa014cc9a68291421840d3a95d48015a07e31251c4197dfe3138c0e4719d87db01b7da1" },
                { "mr", "df45bf1d9cc8c42104caffee1e2d37122e4551cfe38e161cf4c14d03137374756516ac9801a4cd610c8e65dc4c0f60d260ddf5ddf7c8469a06b1a3e2019e3c96" },
                { "ms", "e73f85d693a3edc65ecd00b9fe61c29a665a606c91ece57cad753d157d473416f3a00b588fdc3ecc75c8fa949837f109d9e8156d2cf3fddef70e9c051a684b3d" },
                { "my", "4ba2978a399e38bc1a2cc4ee2b8c6704069a6abc9ddd2f2b8b35dcbf8f01284640ee86dc1363b916af14b415ec01c3f4044be55fab244b4f316ee7421a68ecb3" },
                { "nb-NO", "55f4eb1ae1502126d8290b1b3af6f25352fb00dbcaada9c149bc39ce174695c240cef186d0858d016dab116c7a5d228fbb4a73c7ea6f92a6a30997adf25ae750" },
                { "ne-NP", "7aff26f5316575ddb9b9fe42ddb318db98a78cf14c265c30ffc98173404ebfc2d3bde3c6b7f7eaee9469e63a60cf629986a124e6292225b362b01f09034e564f" },
                { "nl", "696deb34da2665e56ba8674b4e168e9b3a6eb40dd0a4f1ef1a2a2f020594da528a49238ba89ab492d2a0013f45e6bc27d43a172fb89aec2590b9ec05612c304b" },
                { "nn-NO", "5bfd6f14319031372df81341521af990278220451584a9cd8cc97661726b13e064531e83db9683a7e7e8e140345f9d07802543ba001b0df4f23bd54ca1bc3702" },
                { "oc", "7aca705645e8172b43deb0e57aacaadc625de251adb0c80e3be6c7a2e85411889b28cf84c6bf418c15dbb7447a78cd8ce579581a270afeceb5b62685cbbab1a3" },
                { "pa-IN", "37a1c3c1cd01400b07df4c6c869a99e7e80cf9bf23ec023571d4daf8f81a5f25ffac429aa1cf9ad333c0460fa1436ce575c58affc4ddf7df085f87246590c585" },
                { "pl", "2599869684193ff6e2d6002be8a2d1e82543548e956785620750aa486aadec001b61bb9bab389147cdfbba0daa6942ea46f83b6baa71233be36346d44e85d08d" },
                { "pt-BR", "18acd9badce7b013c517e7019e748a66cf0a9c1cba3c76d849640fec8f5708e52cea0726c0708682a28cd0a346a215d241ee6423850830d2e34b35eabffd290b" },
                { "pt-PT", "9c65f20db99e90e21834a2b5d5144365048b16613b58993e3205de565b98acd30c3a27b0fb93b9dd41fe542b5836ef4433767d5d21ca8d86cc6fbaeff379036c" },
                { "rm", "391fa889b4a50c36a012beec82d5336d969142704e06c8b1392c36c604df4237616ff57cd187abff9f8e9aa4c513d9fe69bd22b5738c693b9824ef35617d2dc7" },
                { "ro", "03c71698f8b6428edec7a0a28f8a8907bde960f88e9d31b76eb0da84aecb0cd64a4ea8008822d2e49045ca93d58215de9c435063a797da6b5c8968a30b2c47d4" },
                { "ru", "c8ddde58bc6172ebedbcf9beb12d521a63495b94280e97e39ade27128eb65e7c1e526d358a79b1181c9260ddb06485b89040d06d8cf3d03ecea89be4ddc381c9" },
                { "si", "802f9d56cb34e8b1872009619700167a63bf05fb8644ddb1cdcf17c2b2ee617657a2e77d13c27c7373457eb3f2d9b7da8eaa49c66fc7759da8578d16ec53e928" },
                { "sk", "fd9fa6e15ba8ed77d461f9fcb2686232d611bfe5b19cfd31fa8cfcda7626cd8c675a15e95df32af94dd114c7de8b8cfe1d803da8be8d8327f1e66743e8c1649e" },
                { "sl", "e34589a0a6cf807132d98339f6deeb41d4715f972c495f148a2a20c8e0a79f21b17bbef3b7412379ea01f82d2e2c0a0bc2899723ccf1972772583612950664c8" },
                { "son", "3a49087c221ebe288e2e6fdc8f0e42cdee00c3c52706621329f987dcf7dc0d05bf979bd41f5a4c69c3127035da8f40254efa520f3fc8ed7252c754f72cae1047" },
                { "sq", "1d0329250d06cf1c19961f1292a4fe3a891f4fc5f8230af644c6c37d5516fe09fa5f4e2dcccfac12d40377adf1ef204d43fc2e3c0a7dbd34bc811203b61335e4" },
                { "sr", "9d6f9a9a4865b7cf6f3f1795bc7330f9c42d383e3ad841415f327bb43ea1bd7cb46b76a699dc22b41aba5cb98a6e5946538016ffbf11643db29abb0ea87dc5d1" },
                { "sv-SE", "a4f87f073ad4c9b3faa8f3e294bfaaac0fff95a6387830cd5fb47ca9a5b2bfb3e48af07044d4bd9369306d896b4382084aec28b183f2051e2906bab4723321a4" },
                { "ta", "d8b38850f633be29f5fc37e8f97be34993b0dbf987d74bc7fb7d92dde716850ab49a709dc9ca49f95a8c2ddd8d12896ee1ea46439bb4f22f3051f8bba554bfc7" },
                { "te", "ecfbbae67018e06adba3ea28d2d1d4dbd61af0c011998580c00ac85e8012126887627bad3fd34a40d265358913241ceadfc6353b970337a86538bbce61209ec0" },
                { "th", "64b1c23ee9f490c7126e566fa77e2c97bd43ef4bab605d602749cfe819297c8d8231e5fb568f8f0953ed51ab6312dfb4d15247fd54840234accf716dc65e00ed" },
                { "tl", "7369369c789af1d5ef150673584707bcc61d10522f1e8521bd76789047e748fc5e088129bdae703a57c1211ba195bceb20f43aadfb8684e71e13f6f3b9933766" },
                { "tr", "ce51024f090d82609e88281620313d452d0cb0771c7982408d5d3f968685dea6eb9f0d7175384361516f8cb9b53156f715ade6dcfea31333b2011d5b31776e83" },
                { "trs", "2ac56cbce52611ced798dc39810a04a60cdd6ec36f52d1593fc97145270b4ae973988f82be6dc3ea2d189d9bd6656eff4b9d60a1d639eabdf7cca1da0b37ba69" },
                { "uk", "3c05d9d6c0c02581fa01562e103d2f8999763465c3b3e5bd21857e8aa90ace7bd000ec179b47ae0317643fd184bcb5852aa3c4580bcfe6ec8866f5f25a508b93" },
                { "ur", "faa9885177a3aa68c14d255099868f795553d18cda0092c6d5df887cba50e4c2fb2702d8eb2af119e5107684bf5cac1010e9f25f98cfd2fb75a239b544aa20f9" },
                { "uz", "1aaeb59a1eb91ca6e838b370e7b8ec1d15200508070a6ea0ab2531c7fc781d51d77bbe38f0cfe10f84cf451b56daf4e3fcf2817bc50f6e957dd4d6885340aa68" },
                { "vi", "f0c56f9656b05c7bb959caaf88f78aa40fe9cad0525409bd148942a88a7096ae8027b941ad0ae450cd570ae71ff2107abe853d5d3597c25d8b04405a1f6add37" },
                { "xh", "2ad5904fbdc342c33bd6a36d056705d264e89e1950ecc7ceff3efb497547a1c98053175c565baac37f05419bbfea5d04dce67687e424035baaca08902ab9d635" },
                { "zh-CN", "1d83b64152cd6136e37c2d4baf7a090d4f3d11b4a411a6df00870e30990460f4e8c76c34d9dd7dcd6a04c98d5fd5dee7288e1f036d6e92948930bb701a2447aa" },
                { "zh-TW", "84954be653c1f3d61a0e693e77be5c5825c31630aeac72eeafe41a5310075c4b3b36c7199c33b7089b52dc38b0873f36e0a38579ec27dbdb51ed5989ef8de3e9" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/86.0b6/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "22400ca5d8721a0f2a3235b60e23903880ae45c86568fad79e8880a1ee4f7873a652ba35b3d6950dbd98a48c2a099934a4d405ce380b89313d308dcd918dd8fd" },
                { "af", "90611d22dd5179e9feb4d66a7cd05719b3e945f209ba8b4fb2c7232cf9f29ced5572bb998da85db5f92e70605637546baf770cc379c500f3cc4357c8aa300949" },
                { "an", "4a611cc4e4c5772dff370d9f1d969819728b66b16a9c286fe72b70acf4fb81cd07e5f33287634b2c6c0b04bd8a3a25ab9c01eaa9dbce10cfeef43070e1055274" },
                { "ar", "ccb1d30597f5aee68f101129fd241fa98ba4ac0e9e221ff0a5bade45a2704917309c0e524e7918bc720c445ee67f03a17212a0374ded158fc58f1ab11e66610d" },
                { "ast", "4901b3b7ec6bca2ec76d96ec9045f45be2b0bf22171c3b7b52c54d34286682270392ad7e90fd1a2201d8b5c577692a859244406c46e448ad4f6b420a08c0274b" },
                { "az", "94c26c65805496ec3badbaead1a0f638891f181a8ef1aa7395fbb795bb26f58262e0a50db135db61ac10774f9d89befce655ef209686fbbaeda7ee0c3e00acbc" },
                { "be", "3fe08a278ee208e865bc2f36f0088a3262f67ca103f71eed562c7bfe4b9ff36382f0b3341bc197548c629cda834a43ee5ea5de8b79e8301308f7176dba0cff2b" },
                { "bg", "33d0d5811769abbf4b270289d163865356db751575d9d9c9a7df8239d002b10393b309fd363adf04bb95f59c3bfc2b7ad92e0f1e87e9740546630ec9880c6552" },
                { "bn", "716117187c4ab5504531893fe8af586162b9549675b101b7d7748d28782a0c3592ad2396bc72c441432af9f197698ae671a4ad1ab346915742073ece887a4670" },
                { "br", "fc8b2d75978212fad84859254a86001e425987359f38b1bf3a75a0e2ffa0b13fd996d4eb7ec13291d2403e4d089981b84dd993f2db52686ec50a141116751315" },
                { "bs", "fa64cde1c771844becfdbd40a4bdc777b090e7b097fc9eb672a2cba3e979322392866b731991c321c1a90961085d21ed80f57983d5e5a07db4ad305844d58909" },
                { "ca", "1ec2c9e8b924b25be18b5d14e9d99fff150c896edad5bd7cd94fb80c1c3e299073752962981195287e7c6cc98cf97694751344d0ebfeeab49123a73830d215ee" },
                { "cak", "ee78aeb099586e9c754214d91234a19cae0ad8cd3aa99119520be141b12159ab738c582f1b07c4a9c1d438b94880a3e233814802181b05aed5432c4cc4cef27c" },
                { "cs", "26ef58dbfc585b90aa50460d6a1b15d5aa9eed18162bfacf681e0a0cd8c66433ef86e1360b16a55a94d5dbbd47717f622a6886d0aea5019b01462386fc160da0" },
                { "cy", "ccdae614b8fb558d232d27e4e92e383e813d8c1f339f29fe5456aa2dd0cb86bfbc453cbd809103ce7c9d10f73d1babd050197873b738b71e372865f384487027" },
                { "da", "e2f9be39bba4ca7b6ec83f408f7e8ccbd6a1355640be56189c95886f909f48cb807058488a19a42a417cdb0de6268f152064f5c178799edd9cd804969059b66e" },
                { "de", "1a1fb9ce11f6075d8486ec154a615df0d00371a5af665505a4cecff1c4663d4ed039333e112429e95308c1aec403710dac7d10e36663bc51b4ca5b288f44c6ca" },
                { "dsb", "8057277d5661840a087855f64f351a349d230fbf29bce08f70ea2d0b5f5fb445e84c4b14a1f52bf72f62de163cb1b8f9488e8ded29b1102bbdac167d398c786b" },
                { "el", "571c2670fcec45eaff56dc9658bdcb96e78cce2e04f76f257ec2e5d0929dbbe724ec5c211ea717046efca13158477f6a1a8421efa97265bf99e99f9ec5cf702c" },
                { "en-CA", "c4af9d7c3684de5bf2814dbe6e0f7e840e14a6ac4a88f17a8e743920a8395a9b18e54293a40a4795c67c657588474739edf8d626afee82684383bd3f94b5707c" },
                { "en-GB", "0965ea9fa4136627ba58c8796bc6414d857d80597e7afb22eca31d277fed9a2a147a17548df34e6ff12d805d307de3a69dfaee7ebb15381700136ff834d1cffe" },
                { "en-US", "9d69041adad623111d7096633c7a962eec50eda046b8ec26fa64bc9c3f568e06b07b2979fba009ae1c95a0ce3132948cccd6aa98faae6725b6e3f9337adcf801" },
                { "eo", "a271f50880d71b5f8a51096f92910e167e6fffa601285019ed58e6a77d22ed12621dba1cfeb7e998dd828cd6cc45a15dfbc3bf495ac96c44a542dc0a1c0e242c" },
                { "es-AR", "e04877d04d1c192f0cfa3b5c0038847db9b2544f4e5f1f2d0f96aebd04e69ef7cb2472bc4011778206470d9848e6a65479b7a7ce8bd3bdb4cec0b94cbaa5b5bc" },
                { "es-CL", "93f326cc378f5cc515a3020aec03458bee420808b89e1a30b59fa132031a265e70619ab7c53cec439bc86ef87a084c41f4f9b476d4cd7e62232fdf5d3d79a73e" },
                { "es-ES", "064dd1d9034a7062bbdaadff751e07f79b2647545fb2620bb58d18b1e921e42669690312a219284a3f205514f901780b5286a1f061f254853958433e4150de41" },
                { "es-MX", "b6a6a11f42b81e72f34f3e7fa47536d07f2a827d8b4bd7d5ba1d89f95b97dcda59c267833078a3195ba4a385637f80d929f50160467657896f9449f196f6b63c" },
                { "et", "d17d911a7ee85d16c64eda99114a26a259e6917e1bb63700277f933ed9611c866b9341e3677047b461d01f971c6cc1fc957a0aca45f8c9a4656b5cc1af9beadd" },
                { "eu", "5eedfd9514158be4edd1ce3b040c009d63c87d5aa9d65d07e43030a25bb23e33f9a323730cb81d4d9dfc76bd871b9be7fd3a303ce0c71af3b506d18b07b629e7" },
                { "fa", "7956a04c049f48838cac629971fa4ab41c8a2fff8e511bad0ff1525297560fa7a40b0ba8e1a22e30fb3d41c736beae8ecdd95b70c034b20a9401e8118f119478" },
                { "ff", "35e3f4015c7faa745f000459490a369632dedb3907730f3a96cfbebc642a5a446e3ef0160671e77c2b44c0ac6c3f274864d88ffd70be4aa3075599c34824a479" },
                { "fi", "6925bf5f9d2c606579953a88ecc00f596290cbac861e5da5396483d686a643abb57e64d3dfe27430bd2621dee925bd78d48f11436d0e585ccf1c08aa9f1fe21c" },
                { "fr", "b92ad708c30eae589d9738586c9b73d89a7ff9f4b7dbbd0a08477679633a77876c1ec78e346f29df16e328c17e3ba161b215d62eb8e3b6e2c4e6948c16a9fe4f" },
                { "fy-NL", "0d4d64bdcaeca3e3970990149a34e1e865195a68e49a9c4411e16fbdc0910f26090545c8244121b4542b1462418733fd7aefb5f6731f575f8cba57496ad5da79" },
                { "ga-IE", "26536af259194401aa64bcc3785246436a27d764c054fa53afbbbc38c0905cc54c502141fb5531e59661d06add3ee5052d674f4163e11839b8e0048dad1ec178" },
                { "gd", "9359a3277d2f53c78ecb4f963774f33c7b19bf6afda5ec7970af063bcbf6c94e29610973e4f19a65c331b523794ded2c0d1231ee6066f827a949266eb204aa4d" },
                { "gl", "76b2c6e48560a90d53cb09085ce33fb94dc160cdcf1b217e38459728f84d5bc8b37508832993a5e095512a2b63868c42263cbdf7a60698d13298230978a478a7" },
                { "gn", "1a3a8ebf99ba241610a8f4ef8cfae325166eb6097ff2cb749d184497213c8f0e50701e0c632d5f1d395ebda3a9ac2a4ce4759b511092a6589ca9dd9604649b9a" },
                { "gu-IN", "d36f475310e8d6ad26064272d2bc78a761d00cd75d754425f4763b3cc47857355eb4c8d8ad315597d63029c8571c6fddbd6eab56186d68c15d3d8de8598c0e17" },
                { "he", "413c926e92bd2bc69be56536c222532ea83c5fe8dc8464c1d0d5b4d985bed64969862b4c5b08b3bc540e2e7b9b0c32e8be73a313e62d04f90e1b6bd79a1ebc9e" },
                { "hi-IN", "970833a404be9868f879c97cd5b372ce06c77a557dc83141b7b88819870f88006d11a278aa70336abebc630657622600d3d4349d5ad1f5ba56fc306acb28f451" },
                { "hr", "9d657cea067450419a0ccbd51eab3d6a04469eaae35a91e96ae9356d63a627286d119408642d39f187720ec3f1ee89dd670e2f167fa7971ea1f4869f34d4b312" },
                { "hsb", "ba466eb37003453e9570dd3e472b7578c2789a63fb10022ede6b36601f76dfa70332e5e1a9643a352d22fc7ebde7ac86436c3a6e4ddbed790699e7f93e44f996" },
                { "hu", "062a4998a48b86136745cb3d6a362c00f6e9e743b2219f026fed44e5dd6e9b14311c5c171c7334d3c090466c85871d4c09c24d3dafc81f6868c229fad3679fdb" },
                { "hy-AM", "56f6f4965b157029183ff233b2d5bd3d5f084f0cfd9d4d893b75540fcd425c9ca4727de285817c088825c68c0bd81a7dab6ce70f0743005f7e82177d68ce99b9" },
                { "ia", "eb21a94c8a47975058d37124a3fd9e382f570eaa5df99807e652cbf966f966e70d54a5bfedf72151e5f5705f7cd6d626f7dbab215d876146fe4712acc2cc75b4" },
                { "id", "f4355d593bffca3ee6dae71c8e8ca975b4ceb5a635e2df5ca46db0ef5d47d606cf4ff942642ee063f6a7974ccc533816a5c96052ed0739730bc5ba5d1c6d7b58" },
                { "is", "1c013ab5e08cbf83b44b1746261550edbdcae4a47cd6089052e559124764d635ec9d87036332910d9ba38799c90a91b65ff61dc65efe9ebc513bb1d2141a7ca3" },
                { "it", "0e8fa90fa15a406423e2b67804870f55b9cfb93e7a8fcbc944bf338a8e334ecbe7401bb7ad3494e21a1b7d717f35d82472b865eb5f9827bc7d5d41972c9f7c45" },
                { "ja", "534fdeb6d801433b1cebc704e09f6be52e0820260c04fd562670430fb1b94ddf73af2e87cfb821c9b350a9857047aaa277dc60a2fa421f9b77ac92305907cf9c" },
                { "ka", "4347b0e5c0e0fefc98eedf872c175f5809ed385e6b9d3fc53d29dc53af883b798092f5be9c1dead1d597ceb7a7d42027d318d752c8dafa5a0651ed7136278e16" },
                { "kab", "194fc7deb123c3c6b24554bb0a381757109950257a73eba05a06e3bf70fa295c87fc6202c10bf09a02ac36bd6818ad6939ae362a9ef2501ab0cc46973e787c9e" },
                { "kk", "cdd421ca44c466c7ec0c404f00885c247afad4318e4690e518c011f8f0093efdce3e629c3a2ecd425cf451e3b3f85068ba44600e86c3c4767276e7c5adc7d4a6" },
                { "km", "89ac4aa1de1a3bb07f5574c74291d4ba184c07740b757435c5f006ba1e4defdd931e1a3338cba3e6c89752ff1d7d5e048b06143dcb1eed3ecc698cfad1a0b77f" },
                { "kn", "c5fe3fc62a6ae0ff0af6e4e85372c3a9149beab244c1c818c9cc7c6c688b722d4329fde51cb7af347a0eb591ef781941266010eca3898b649c568210c7cd7e39" },
                { "ko", "68332ecb26a9590580b1014791c14095a2ae7c8f64f48b8108d986cbe8ba838c16944375d4bc2d1bc9eae8199a68c3996a8a346d7cabcfbc245e18497cb24be8" },
                { "lij", "f7d57584866349c781ad09322a34bf51e7b7ddc1bfe03a8abe1ceb8118d09e20ce78501fae7d3e769ef50aa793a4cf43198a519783adf5675d2992884e592b55" },
                { "lt", "358f899f405eb46762ea603e3d2a40ab833ec312278765691c20cd791efb2e9e648e9a7d33a564f5a99eab6c85d88afcb76c8f6915009b7a1aaec1ad31018a51" },
                { "lv", "a3d3e5b847109dda27a29619e150f91f110ceb65c63306d9ccb7fbb5601daef277761ebe6aed34cb09359672d13ada541c4c4eeb13252a161332424be549dc80" },
                { "mk", "ff0c6eb1c8e26a96be686c063bae32d926968d21218cc7137efbf581fa5fb7519651820fa310b276defe759bff10a5fe93fe1cbabd531194a47b8f047f294935" },
                { "mr", "5383ab1de11e0ddfa3d182b75088a6bae6bbb78cdf94d7bfdcc2c526cf43cee7ead2de1039ddbb114e214e231b9c5c3f012ccc8f4208075a927dc5d50c26cf70" },
                { "ms", "28cceaa5930ee303a4167832dd4f17fa2b5a35eb66f642b5c175666a1ec6ed5dc5797f2fb9a62b739120d101376faa88e2c802e689edeb85631503547a6713d5" },
                { "my", "c0938062a0b7d539c9a28a3ba79c9452fc30621bc7f5d9336de1159471ea3145f50e5b2cd3afb49f19b403d837c4a8d49e69c4e032fca5288d80430ea789810e" },
                { "nb-NO", "c9ccaa949c61e830a3b805cc4d5f3aa73c9ed3d108f0992d2658a4cca4e55166999fe01d6896cf2bb3219438cf381360f80df8cc8d805639cd5c835affa00905" },
                { "ne-NP", "4eb848cecee4679832d3f8832dab4aed031ac50a2f8b7db6c46f4299ef04797844cbd56285c2505fead1ff2207066f94783dcd65cb468c8055958b9d0e4fc829" },
                { "nl", "e40d9e90ca0e010be17fcf2d2574f2d43397a97962c49eea1fef73da60f47d66db7eedb0f27d632f4a55811533330e892ae0a38f8bf85ddaee5bf6f840f83bf8" },
                { "nn-NO", "471a4bb2bf21b9a2927840ae06de1ad4b00e60b9ebd14d118643f3eb172cbde2fd32deaf83e85bf8f36cf910a1a539558a94300717920d46b2e62d0892a76f5b" },
                { "oc", "7c899690a0e822c7c32c86a3d238ca47e026e9c98844990e4192b06af49c258a8c1c4bb40d3910da5bc87f46917a66ce2f68521a541949ef5708bf3f9528352a" },
                { "pa-IN", "5e74d610b1943134fb28452e3551de27f8ccec5e72fd84b732c9ee3ad72395daaaeb6599202c163c4e74720b1ee02e0a43e3c3d512bbbf60ae331ce7af884844" },
                { "pl", "d84fa90ae841cf3fb002a6be2bbe1700d3671d82d29076d0315dc67b58215f0aa0563f057b969c69980c4e00e35c5da5933f4b3a38e04ec1718a859b01540f19" },
                { "pt-BR", "d06ba600c5660bc59cc19c7f85cfc3dd75071a423d6b6670eb5a95b4401b84d573f66afdb96077deed266ae08d9437c96e982b86b15ba575fda80e5ef81101b6" },
                { "pt-PT", "f1addad0df81069a6145120c5ec09ee739f3f287bb314bfcbb476c6a4fde0417b4ebe93e3b5bdf8f3d7beff296d41041baf88249276cea655972919802955fbf" },
                { "rm", "4091775348df9893828ed7c952b21e5464dd90f943a631caffc78bc4e14817876b1322f205ded76f7575c05159a64f4c16099b871cf200bee586a97e2a47e291" },
                { "ro", "1ae6b24e356a1d0c3fc588fd1004f7285a7f3cce680c12266814c85d792e3997ebc4768c2da1792e7a183bb12ebdd8d05aa32f199c0b495490f505a949aa24bd" },
                { "ru", "d836e1fe1ee99a0964946e6de780fe1f9fce2c751e8c85c78f4318923fbd64892073e1817ecdb111a2bbf7908091f07c3a56707d14cb5a3aec19f6b74772bf0b" },
                { "si", "b9708cc9b298e7efb028b8ac1ee020196229ca1e013d3af9ce8103ea570793ac7567deca1aa27de159ad949c70945d98974fb450bef2bd43c69ebe585bb5e76e" },
                { "sk", "8a4145fefddf9a0280fcbda33d4ea2474b1e45e048ae8e091a1f90d64b529174038933613e13aa9d8021a4a8f65df9bb9e243ce5db177780acb4828c0b25c2d4" },
                { "sl", "35ff7dcb623a5b4e4bc9837f59eb5fcbe4159ccb697e90ace17b9f68391aa0e3ed7e4a21b27a06da3402f981768c670ee03f21d4a2e021636a120427df367c19" },
                { "son", "b202c0fc8435e760e9a22d4dd5ab472a9b42504044cb1ead2ebeca6ba2f7aafdd5b6679c6e792d3c5b5b2a9268178d60d544f9b5097c23b629148450b61d20ff" },
                { "sq", "89a62e45718ec2c9abd0810ac7f71341ed53415b8c379fcef15ecb9ada89a9cc6842e17f69448a718f1c57134be7a5141282bd6ad58035c4064aa814370c3c29" },
                { "sr", "dfabcee2f262951385c9efdc7ba03f1902e54442d4e79eb1c8944bb63536d360c8654176e286482843c07a69895bcf41c12c677631a3f44960200db24ddfa0ef" },
                { "sv-SE", "1442fa6a305a2c205094f4929e8c77e8ca99ec515c6b7683394aa8b69e251fb6687c48d77247840625b49a0b6e22b108997f791ed147ebacf118acb1230e6a56" },
                { "ta", "f4aa2f291f43b8a884f2b24b0fea3bf8476b142a096a7fde637e980a3255e14f4c7f342c6bf17d49733bc1ffe6ce098b4e1f5ab76fd4c28751395fda27f9661e" },
                { "te", "6f9fe0ddb05f8c51fe6a2c0d0da4fcebfd37b552aa51a537cd1c723e4cb8f59229faea174b2ad28ccdc3e5fd6be1f58d80d3ffa5cab317ae048b0fac8ea4bd45" },
                { "th", "c49f2eb571c6b41e3c8da35c2e2a6a84f74b91b6d26a30f6f9d0e1f1ae1f95735577017734ada6be2242e27a58fb8683eee596d96fe0e80d6d1608e8e68dc434" },
                { "tl", "16d39310137eaae391e7838851fbc880f136163da360caa48611b089ab946480032e661182646c2aaeb90e024634084d229afe06d39b58df88c1f4f487e9bdea" },
                { "tr", "7765ed0fce889e2edc35545871a88ea39d039b7d84096491cbcb427a37ab7fb6e80a4499def3571861d9a9ef0e1a169960705e9df24aa0d39216999cc7a6d518" },
                { "trs", "5090e4454309cf1150e816d406e0beb128abe1d66a705b801392965c00a96b94911495a406e6839515ced85e1a110fd421c8ab8bc5fcc1cf6f056d0e4d66f276" },
                { "uk", "ba903aac4f70689ee90afe67b336c25b3e5b6f968c47f886edfb80a1989665d74fe330b1ec62b4235cd04613a66a3734ead4f293bd160b050bc0ca92b01543cb" },
                { "ur", "a4066fddb565ac470d1bd3331a6d70a0f255aa0fee6c190dd137bdf110bd2dcff86e24bd83aa8735b56faf2cacfea1bee5090affb85d0feee45d5026c4e8bef8" },
                { "uz", "711a59dceb26691c56dae142bba1f4144ae9defebb2780f7225ed14635db8f63062f300c2ae835e13ed7a867e41befe6bdce06fc4d58b23d43d8584fd3c512b3" },
                { "vi", "010df4597e7463b1c412568b3fed90d59b722702c2ab1176703b9f26e24e4acb32d6f400fcec42b63ae3fb55c4fe35ed494905953466232b3d878b1f8effc2ac" },
                { "xh", "3bf233c28e047b131007778b60360428454dd30608eda7cc133e6385e26e49cdbb1e2e72254a117853ff8af940e331d80d1214bf7821fe9a0f550f391a6bf5d2" },
                { "zh-CN", "5031056056a02eb6af468ed7eb4bedcebf6e976af4491bc67e0a64f88c415b9d8d0fd14ba5fe9b99c99c46c64e9cc7190980f0ffde075b4fdd9868b958f95990" },
                { "zh-TW", "2c555c5b99bf52784dbdf7f2f98c3f9e9f5686297163267caa014ac9b35052eea3407c78ddb1aff86ebdf3f9787583a2f428313916157b628acf876086f66f57" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace

﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Release Engineering, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/60.9.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "706f66c0958a6becc973739f888278dc5faabd8fc82401a074913e3a407cc069305439c9bd85a655dd4236ba521fdbfb0cca68a02f381fb170fc36087fd6b273");
            result.Add("af", "525a403de0b48dc6fdb8e1cb0fe73574bc087eb4f110263bf628d48b796fd704d37d25da55b8bc4a7ca778eb9d8a928af970d5cae1f6f59f81a47ef8ba58819a");
            result.Add("an", "d1762328049e695c8489ad241b4007f2f185756b8ad5769c9a5f30b5cb2ad31f6fcb007cf87f8a91fd0fc7f1d942b64adc42e5890db4fc9a93c5df9555d0137e");
            result.Add("ar", "97f233b9fcda972de1381f58c7d470e4547a1faa813d1ca25e470af16723010aba341b83b907053d863ce941623838760833e8bc15949104db3a52e2740f9877");
            result.Add("as", "545b1399b5756975f260b73a7656beb58c135123be32516ef4729b437c365914b16da997da010985b705bb80c3e291e6875fa70ecaf57836470acaa230aa5ce1");
            result.Add("ast", "6af9b2f74f8eab422280b1d4863e8f947d6098fe4d34cc938b9cf6cc06617647aa86691d0b79200127efb222faa176e1159813093609f37a4500f898924fb3c5");
            result.Add("az", "14d2929ae5398ff99e598e894eaae57d5e8e50a4f799cd133e9a435a4f6cf50b33b7c0e86acae555c9fc80a8252a25cf4834a1289c37d1c02e4907ce5a68abf8");
            result.Add("be", "08f44a90e6d8b8bbe22152dc8f8c3cb92bfa1f88e6480abfe308b6e8d8c554242915abc38ad1019ae9563eff591e6e5411822daa480c08f160bf21f16a5d52dd");
            result.Add("bg", "e85efc8f2e7cecd1352fdd909425199284ed73ba59ab690f5b717b5c963ad86880522d3a025145462b322d00b1e44449f7d5fa2466f0fcb2b8545f1da8e58982");
            result.Add("bn-BD", "f6d291e20b5879da1151fa4671fe050da5f041a60df891b090a4812e121408bf2a790334d23b5b0831223381e37bdf33067dac942b8e2c8e0911506e1fac00c0");
            result.Add("bn-IN", "fafddbbffa4aabeee009ca827a0aa038b4bee2fd9c845f362062a50bbe688adc26e0e0b55e5c500e5b0f48354ce03f7feba2d24a02c88374f9184bd8902a1f7c");
            result.Add("br", "d83838153d000cb7d3c241d141ab90e1f41e3efadf65fcf670fbc468f1f716b059f1359c51abaa7fe5e5232b211f01d1e5b753be79df84a5aef0deabe716be1f");
            result.Add("bs", "133147fd68f66a8c9adc79be3bdbd1cafdd64fef2b082a3b4310b915102f8677ff30ac27e1c3ed11a591b7d1c233ad57b9e19c959a390ece04c28945fe1ea0a2");
            result.Add("ca", "7d8e528412c0ccd8c023a3468598bb858ae47c458daf64f11c0596f6bdb1bdbfcd5981e25da17625f18d6cddd6291c1bb5e685736c64a3fa62dbb088cab024aa");
            result.Add("cak", "065576c6dd49ff5e19de624d5e6fbc074ae82b84a6a3ca33f093d56680d1347a39b8fc1ab89b5dc24835863dac51f2ab208dd0e64e2478c0bd0ecb85314b773e");
            result.Add("cs", "c288706a082a5849f374d705ab0299316a69ff137cf30f5b024f5fa9f72c3304bfbab24a5c98f634d48bd653ea6302601d7f096516edd38244638a4809443173");
            result.Add("cy", "196d3bfa47688f3933fef973772f575e3ad035260a2942f6acbef6196d4523307d28ef40decd19eb9320039406324bd75c363c5f9aed269d4d1d8ed45aa7b1d1");
            result.Add("da", "73f7064dbefd629079a6ac7754ceba9d1b9e364bbacc637c381bfa4570acde824a30fcd72ceb9427790c83249dc0ec3235debca984da4fcabf50518b3473e026");
            result.Add("de", "ccf75a9b5583ad1677d50981996328d6e63671768bb03a3e7be335f333d650fe5502a7019d8d76a49419c2d2a9f9fb06f51882b7ed9a72daf3f7af374cda6be8");
            result.Add("dsb", "6370b1bf78d85b666a5176ccc9929b0000ed38ecdcf3f7c1baee2300c0b668ed0d7bf72cf13a95342ccf867996980e39aee1a9f6cd04825352c488cf599ab0d2");
            result.Add("el", "1b459cb57a4940a7a42666c80cca3e85730707eacf1c3df18ce92f4da819a5e52cae5fc44aa30fe30f50a666bc1c60bee082cc44b135e468ef5b2168c8b776fd");
            result.Add("en-GB", "4e68623472910acb4db2306f0952fb49adfd835888a04525145ae234524a3873c43eea6ba4a336ad1ffe4ddb707af9f940489528e661617d9073626d549ca4f8");
            result.Add("en-US", "7bc3b835dda67308f1bd71499f00a51e152b63e7dfecaafdc7afd32af9d0973a4199fd2b02aa19c277b539e273ff8c3241f1d60fa483d6624104708318fc5e2a");
            result.Add("en-ZA", "acc38bb9f6ed040c6a8901576725c72ca722e7e88eb6269387ed1991e9dc707e3d4b6eaad08b1ed1933fa6b5b16677ac0d7f23409a77edf5466225211db3e12c");
            result.Add("eo", "ddf3b77abf6423d8349f2df6790e42b119bf9806d857ed4b356c07331f9e71b5b33db404ddf47845cfedcaf55db3325d8b232ffc86d57863fabb41533f7565d9");
            result.Add("es-AR", "f129365f9ea6627d4034eed01b2e012094d8e9bdb2751f3f3828a52028e7e85abe8d237bd9f5eaf3b882c6aa60e4a1cb4b8ab44a25ca70a3d5a2153a635a72b9");
            result.Add("es-CL", "1dfde41e4ba72cd46fe83b7f67b4e21b8e154ef6a1154200a1c717f0b0e77974331b5d7196711c8783dc39b0ee8d49e3936d7683722593f9deac2822f916ead7");
            result.Add("es-ES", "1a28e5110eb1db34205c15b1fd25f05a048a72e9a238c60b49296855d86c9b2c664f82b0bb50604430a9e54e6306176043206d63557ebdfe870f2783436cdfac");
            result.Add("es-MX", "71cf5f996e69c8be74213d3c3626d81411aaeaed35f4fb63462cfdb96ef901122c6a87d0004315673b9b77e8bf29da08d28e139acde59310ed686cbafca3e2e2");
            result.Add("et", "034b4cf37b902ece1cc75a6fa3876a9bddaabac75ee92bfc68a962abac57cb447dffde0b192d585179f71d8490958b13ca82adc35201cbeb93835fb96badeaf2");
            result.Add("eu", "211245ddda00e384a1ccc199c39e4bf22007075119942387da2d12e509a7b15f9bb95c89ec4123b2ac3e1b9cfcd4b59ae96a27243b79e94e62b8693b9015aaf0");
            result.Add("fa", "6e33d6cc0d55338f021b4e84e766f13a28ad8f2ba9de578a1aad8beb5256c916e338dda2ac7c77554c80954abce7f48ef37721bc307d172f05acc9b06d376a0b");
            result.Add("ff", "21647b5988c0ed84d6f48b03dbc836e6360a64169f3edf59dfcd42f728dd04d8ce31569d21a4901b060fba54eae0acac6dc409b4efc65df03ffc69b870d03489");
            result.Add("fi", "9c920f799d5e4515b9e2ca4430e52d97352539e280942d9a0a37e108315d9281b9eaa7bf94bfa077d8fc0f117080833f88d126c68a2709b8b285f9d85225d431");
            result.Add("fr", "063da9c6e81a4c88cff3a7bb5ec76943a43fd9813ee18fa54ffc5f9692c3191148a3bbd40d1c6e8e3a984c515f64b570a7ae678bd53bc3a08ceacc3da54673bb");
            result.Add("fy-NL", "408b8be7b732e35ce18777b08caa3169ba082393532aca355ee47b1d819c22fa1230f41f2c9f646b0890572094006e99cdac4717562476d32dd1134002d17c4b");
            result.Add("ga-IE", "edfcd233e84befebe4ceb322a2d20e9d41aafc618b03ea894835942c9d480b64116f8a1e4d8156ae5bf8ed9e1292772533bd04744e8416744da7846f80bfdc88");
            result.Add("gd", "7827c89307674ae84e784a5b686241cbbf2effd15d8d84827791303db24d1aebaa947e965a21a5100d864c2126528bb3867d1f57ed4c68dd86f44d80fed56bd5");
            result.Add("gl", "6cd2c68166776bdfbe8bda0c3cc0588ca28e34ef8987c0fe573375ded284bb2feb79017c159e28483ab515aa4165bd13627915424ab129b93c45c6d7b0a8177b");
            result.Add("gn", "99ab36e82ecde992b8f8126db28127ba63e9e449cb6e4e36edc974b71004ca486668d30386c3fe6dfeab8f6e839c9e60b7b1147eb8115041e1443e56ddb5a8bc");
            result.Add("gu-IN", "1baa7edd4d935ba5fdeacc77f3561e42460612fdb330e830bb9ea20de579c488a5b626ffdc24238f91653bdb75b97fdee7ffba5bb553748ef48ba1f2b5ec572a");
            result.Add("he", "6bbe5939f3bc94a05f6dd5e9c3d6213743bf15e5908314ac60530fbd8249dca8448f04a8270a948a73bed84348299b5d0a3f122e0aecc6893733ca68a28cff28");
            result.Add("hi-IN", "4c06ebdab4b1fdd5cb349c0b6e66c836f10f29ea1d4bda53ee63de7adb1d7abb5e45b725a3901298e1c907bd60961c5db64623159caa0cc700f48f7c1f67939a");
            result.Add("hr", "1ba64e9b69ae9745d358a8a28fc58c77f8da8bbbc2c0159c85c08bc7e32a70d679a99992e864a66eacbc1bb9883e76cb6290826cea3a4678fd3bfc37d4e392b9");
            result.Add("hsb", "9d1f999494f7fb245e49ea58ad099ee49de3f8d312234501b85a39f9a39c07f9812fff8617f4689428032b77a25940d0f08ab5716db97976239edf4a95342d8b");
            result.Add("hu", "acd91072c19d0d44d490d45f9401fa8d5e402d930618e5a0c934ac3dc9d69e4483a2753be586b147ac2d8b7027a3429334900201d53e84da5e497731cc72f164");
            result.Add("hy-AM", "77e687c6226435fb468cad849b5b90a2bda18a20df4324879c538d7c54168d9e43d8c7a9c70f1d9d20d711e3103971c50a2ffa5f944e9a308cc831622f34c2e9");
            result.Add("ia", "393f3eb1a5d6dbf78754b56fdbdb60339be77852129a9a6d80b5c5cc5a352875f6167cc026977a9d1546be96abc26627d3f54f43fd31d838299475bf1d215031");
            result.Add("id", "86ebc86ed5f968a10f931ad1e52c9636537381f5c65dfa99c8406f37786968f5be6bb3a41cf871ce9667a9d81d91c9703a9d5f5fcb7938ad53f49ff8914e1e9e");
            result.Add("is", "69ea89586dd78327e397514f01498a3e808c33ad2faf346df2dcd6484a9d90f1329e7e904eaeb36ad2d0c6f3436f60d7bb2d264a32cfa061b922884f5481f68a");
            result.Add("it", "796efd979faed76adfbd093b12e96419b07a0dce30f5223fbb25d1f177a2e560ff1b9883287f6f320cb11407335a52343a79664d9af65e8016a1cc08bbcde669");
            result.Add("ja", "55092fb23e2d21dbcd1701b677f552ae21ffa23205622677185936a9149aae1e603d3e94b15889a2d4a11583c087be03511de793dc3b6b4a8c3b9bac4b7177ed");
            result.Add("ka", "35293d14b967e37cc7d5820d7c1b06fe24edd1271402cb972b0f6f3cde1ce0f37bb3174f0f0972c158008b852e16c2a1ec32208774776134decbd75e1be7556f");
            result.Add("kab", "f4289ff5b7942b73549cc87e975f8c441ca6f0094379bfdb9c54324fb70bb8b3e8822b0ac10d700dcd535cf1f43817fc97afc02068fccfb2aded9779f413bb91");
            result.Add("kk", "b0a7600972298e7a0a344fd5e4c5c68cbb0fba4fb306605e563501ab01c5648e4e7d1e157d2dd7fa701ac1d491c687dff308ec11c6cbcaf055765790a45a6aad");
            result.Add("km", "b22d744c296b47dc26df6e77b82cc2577c560d70e63d2d54dd4a4170a28d0bdaec367941f8ae6f8cc8aada6080aeef49a52c513bd1c5caab26e676493010ac44");
            result.Add("kn", "067eaf5278af19a6091200ad5528d92dcd43517134c20a1e80bfcd3b6cb1800bfa953d6cf4a5a2826aa27d902f1d5dfddf4e817246b55252883a0da95b68490b");
            result.Add("ko", "79bac59e4031a72d144079ba71b4fc3cd576fb13271b00aebd6f955620504fb7b975c270b1e7756ae950767a68ed052aa5398084b9f16ad4e86c2c990bc2e70c");
            result.Add("lij", "b0359caa93220523500de7fd95ef4a33832eec43653d113ca86840670fa902ca7eb0a4d47c535443b13fc227220f02ccb40b06abdf5ae8c01ae3b9e874b85bc0");
            result.Add("lt", "92be5b257e0031bfeb6a281c2dea5ed7e6b06f2056439ee53ad3119ac1bea57a7bdd65b53de3fb97c6b754027a6b71310bac718561e62be15580803e6db488f0");
            result.Add("lv", "5e802924cc76c7b6ef4ee58cf4d9867d46705edef56fef47eddf91279558223c7eab32d717b8c6b86bd07590eaf975fa1f74011fedfa73eb50145e98737446e5");
            result.Add("mai", "c69fba75fef6838001f57483959f627e4617d44ab464df2f3410bfcdcea53728cb409a307173a0cd7d3c6675f24f6bd95b32b18e6348f2002e06fc9800a86ed1");
            result.Add("mk", "75854ec7fb2cdab59ecc6da356f5d3390a03a694fa02632f78eb62b305dd426d80f94eea4b05ab9dab9a60147f38bda608ad88c6020541bb59934b6858966324");
            result.Add("ml", "65bb6af9e017d468cde66c2cef060e3d88a4f59f348609dd72d40d5dbedf8c874996bcadacfd64a6d22bbb7cfb545df0cb047de5ffee2454ad8f101b6f0602dc");
            result.Add("mr", "293d51a652fc5dd197c11594a8f6f1f4b1434256634854998248236f4e4d788bbf1827a2bece66e9772bcf06952249ef36ec8eb0ecd30073ab1232d90cc70e15");
            result.Add("ms", "fc46b63726efe20230d84c38bf874209e11eb3c01d3faf4945b00fd8a864f5b1a5795b573903769d8e7df6ee15554557daeae5d1f818da107ccfee5f0cfc11ff");
            result.Add("my", "10019d8a8f91ad4845781c08bed8658e8b601e10def2bf013bab87b012471ce7199a01bcb80f7aef0c1bdf48ba8b624947d47822caf34e1f9657f88d455cffb3");
            result.Add("nb-NO", "7d0b9dbac5c887b135bd0b7a9b6b26ad8cc22fa361e1cff1364e61dca6506f1f811c180633432bf6cbc38a5506d3f888cc4fab352a79dc22dfbfb2fb919b1906");
            result.Add("ne-NP", "595e5e1c08abf8b6a46ba71f6b655f09a2a7a878ec65b5e50a247384300dc4452bd6ccf0dceb5a707607bf0b46f55b575b08edc353acef7050feab229ba1827f");
            result.Add("nl", "d1085e738b57d495383765a5b6bca60874ef1751205cfc97c29e79b699e794589cd7f05cea1648cb574f1479f6461a4405d456f8ad89b9c8351b6de2ed45ebb2");
            result.Add("nn-NO", "7153a49627b43fc043a169d8781f03484e5d00d070462c1bfd803e81680051e91dacac04c28b1fd5cf3a02d871a8fd98cc717e63fffe59b800441ea60b743e4a");
            result.Add("oc", "0a0ebfce3d284ee1d1f6df415b018da9e63693cf6c517439dff059ce21aaa9c85ed5e24fb2746bd7295f1134696a1cd2adc3584c3c33e753d192ef44c7d0ca68");
            result.Add("or", "40ee7654a43881b0d3517e7ce7647f4e27cda7d63efe407bcbc118acdde28abb6cff13445705748c66451b326e454143d7a42cd8a07c42928b0670502321c157");
            result.Add("pa-IN", "1b119d23e4e04a23aaea942c56ef37f478cd32c8482eee2faa99aa5519f5f32e6047ff7b8edd0c60546eb687e98e67fcecfa516227b581bcb9336032372bd896");
            result.Add("pl", "3a10e3cdb85dd7a87efb8a1ebe461a2e8012f3387857f18e9c2f457e026df3da3de4b0919d1598d87f69c9cdda613610c95752b893dcaea40646537285a1b8d3");
            result.Add("pt-BR", "45e3d7513162644e25c564badecb8a87d379390fe12449f23b51acc9e7a1bf7ca7467ccf1ed99e72ab3c066916d454120d0b80c5b7efe476abb14229809b32d1");
            result.Add("pt-PT", "355f18a6525b2695d5f75786c91faa909097f94eb6e9d4241ac5884a36923a271be0c0f7a58359026c84c939fb4964fb1964a336e19360cddc7c4558a4c3e22d");
            result.Add("rm", "8bf0a916e3cf91c77901659b1acfa977eb460a5009847fe47a4da2cbd57c81e4e03fdb775800f6e65883b9080786eba735b3e9ffc1d63dcd602fd2a661f4831d");
            result.Add("ro", "fb0cc05f38eba10b2ba57b36c00dc8b55c7e057a84a29a1fbd047f8327bf2478444c153ce550231f9b7d51ab17c60bf1fe67503b80b20af6c26a586335d3e97d");
            result.Add("ru", "38b246f5a6ef7b0bf23cf6cc11e6d8063502ea3a5e7ee206d4e7f4a57d644b6758f0a4479e8a0eb1794a548c65c67fb960566eaa91c7c51609ccd2cae51c75d5");
            result.Add("si", "8671ce7f7ee16cb22aa0635b777a5d26ecb9438b2699b032dd4c566fccf2f35ae475811adcef320dec4d905f8de131f512cd8cdd0f81cd4220b0c24606a72181");
            result.Add("sk", "73a186db2324158dbcffaa94d51425e402faea47d20fc4355090e5fac9f0ea620979339a3ab32f5572803f2aca73815f47db115397083bb5bd2089d2fbb4c507");
            result.Add("sl", "d1d8b63dd010df2e9499291813617bd1d5242f6c8790b5894a822666015c547a4c2f43ad16b5d422625c3549b028a95c05e3e21f8d7bb66562b447209305195d");
            result.Add("son", "cd1aacc14834f0d74661211d0c2a87c7581d166ec8caec9f8a11e5749f7265baaf4c01f5a5c0cc1ed0dbad0a50631a619d62f2bd52003ce433213ffc367c56f1");
            result.Add("sq", "8bee2907b71a27fad227619cebaa3c6dc6c6ca1e472b60eac9c7c06a673afb849794b37431bd43753acd2f7992cf81d2925b14654cbaa4a752059b4126d268d1");
            result.Add("sr", "072d88afd13e16cd74066937f3f71a814c037555203b07da3618d42af3fe1c593e62824e225285ed61ce2e3b6ea09723ca13d9c6fe9e93d50d9a4acedb7f407c");
            result.Add("sv-SE", "fc9fdf5d88675e6a328196e3fe48704a2914fc8af1f068ab17b9f870c402b0d0ab10a26bfbd29dc95371e590f677862ab2fe8d53ec80f520629aad1180b80ae2");
            result.Add("ta", "52a7a99ad772b3009245b388134bd6b76c455f9206a3024219c9adb1a6ca42c86078549beadc9600f860344d8beeceaf32ff25f41c1456d1b2f70e66960dcd3d");
            result.Add("te", "d64ac9c073c23d2405b56c1f7435f19e01006d19f70d0ced132b261df9db534a3d539b59bc64a26d7cd0543691d069f68ff5bd2f8656fd00f058c245c0e10632");
            result.Add("th", "4ee3fa3d43694fbb59d87327e8028e4a4c4daef970eddcc93d5718b6fae0e9604c9ad52696dcf56a71061b4d0488c5db8137efbb997d1cff052cb9d4089a97b2");
            result.Add("tr", "47e6a143fb53e0e864badea40bc8ebe2a8386d062e82c4f1b69cf016415c84d59f38d5bcf9fcfd8c6e52ddcc38666c9fffbc18605e1c6631dbed4c6e1c51d165");
            result.Add("uk", "bc04a698ee1f81724843966c74ef39968e24dc23f4cb392d268fd46ca8086c606037f6bc29fb6d3e58b2e10b791b3f3130eb5978ff8e4362ba03f025760d6a99");
            result.Add("ur", "58623d8d96df9de2db0649dd4ffd0764008e02fb9a38a3695d064bed45fb9501d662f68500e439c1f297b0c176e134a72facd8e4484a00b02d23cd32628246ff");
            result.Add("uz", "58078b380495a06d95635774cc1f7a277337a95bf7a3f3eb72e98f9143542cb37d96540c306f5b3fbc7a1450754ab2551b69a154261f268e43f1cd8327d01d5a");
            result.Add("vi", "5fccdca9ea7df35bfec17f4b0c3f7001b2632353c712dae6a071f8a895ece36a6170a177cc4cf5dcd1130aa6cc707f4f9f50f79c1bab13dfa61d138618dd957f");
            result.Add("xh", "d2796e7361daf3010f5245840280e23fb4464e0e5878b385e4ed422a88a940df81ed6d33bf70cd170bea5475269cdce7326b7a858979c8a8acbfdff31b8bf007");
            result.Add("zh-CN", "8249b35aee7b51e7e16d9bcadf203eb56e93efae5f94f8ea3de3f8be41597774e2319675882bae1e4defeffc8e862e5c5f320e2785ece5677177ac61afbc1177");
            result.Add("zh-TW", "63c0d2d176144176f1bbae9234fea14f609d29dd87ebb1b295f6484a7e09c291d58af5eafee3a152ed82ce0b863100a52283f50d105b19fe49f6c5a21b217f3b");

            return result;
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/60.9.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "4b6c5e6552fdf674fc3e683048194f9e6ee62e8eb4b988b8760ffff1d1de323ff40ec690dbfc243c733743c58b3f87704ab00b1128ccb89adf85b51f5bfe8f31");
            result.Add("af", "b253fc396da8d486fbcd5a285d8236fca4deb196fa7f882311862ce117398c4ae97e17ae1f86a8b46f1679048abd6e00ddb0d6cb6b0ec9353e5a9d92ed286a98");
            result.Add("an", "4ec57fc220bd45134ab6a90a5d4257d5698fe397d3d6f6416d200c19986a03f521cedcb2a8921624bdaa8376c431e01631cb39cf3d8ddd0e3b856f767fab43b4");
            result.Add("ar", "1e907d39b694edea9ef0c66b430b141c69c37613cac409464cf2d2c35c36749c0c0f00103db081fbd103854ca973b36e49291d0fa3ad32022c9947be7809b465");
            result.Add("as", "5e937b75ea0b31ca40afa59301914ea0a5ad8f3741f3f52694b59fbfc5a6afc6134196a68b24ac3046d5871753275fad133f4e7b2b177d35ce82d3e6685ca0bc");
            result.Add("ast", "71f830a907efe093a4915566fb7d85424ce362e97df9c704f699be6ebe06f37095ed9a2f182166fb495f0c8daff60acfc7aa63a99422cbd34c37991f8a777c28");
            result.Add("az", "2aaaf429f70809fa018b2ec5ddbf6b18f3780a29f5209004345d3e6619ee424ef9d22b5e6e3017520c91e9e0ab076961c18bea2775ad6abd75c07c9f5740ed6e");
            result.Add("be", "34c8284dd62d8b7e8bdb0be328cf9752a8fb773d5bdf5790acbfa19b0ebdfd1bcd05cd64064f6247212c20ec2bd4a770dd055b8c9d2e85ed353b86da9ec0cb6b");
            result.Add("bg", "d7b178e45d3a5146d268c58068c8bd0fb409ac47b6132b9622e5ad046a444bdc432cb2a4ce5a3b528afb08356b0b0b470475af5956bbc61e8dfb10232d056dbe");
            result.Add("bn-BD", "1a161f0da5f284f842f8166866a7fd3b248b3696f862f536ae5abd48e2ed528f9848c353693058a8fbf516e87dc3305477550b7666351f5edaffba9c5daa1984");
            result.Add("bn-IN", "f707c85188c9f18082e8cfc4cafc12a7ef891dd39b9073d4be40c845bd1ee814aecde1a16170ac02ffba92b32b7e1d77da517b2647e8d96d9ca5721b94ab41a6");
            result.Add("br", "a4bd9a2ffb23ba19f5387366f2b1bb3c8b2e94f566fa1dee2b9c67d41bb663ffd11f9a438f7c7d987cba53d85a713d9f74666fe264c677fab0ee6055944475ce");
            result.Add("bs", "b46f0c57343c2990279d35fdf9723286224e7f87653c976a68a9e9ff37849694ad9f207ca31d7a53ef74ef60d4fa3ec43cc7d00ebd2ba3a277f2d4048a743672");
            result.Add("ca", "75f117e2c5770ad3ea6f39ed4efb9e144af10d8bb885c426ad1bb9fdb55f0fa09778aef3382b2926b3fb51c04f3280be5e45c4fc264dfa0d8fd46df2b1ea2cf3");
            result.Add("cak", "12ee2baa3301411a3099bb9a6c4f32eb5355098daa273be6e22e2dd9860897564d82a76710e4d563af5ac2b58bd2c58764a0db4ec1045b77bc627ce9dad96e7e");
            result.Add("cs", "b3beaa92e695f6a29859cb754bd4af5de26a149d83145ae4c4652caaf98df68d68f19f543ad7146000e559553acd101dbf841bfbebbf4379dbc10aca589f3a48");
            result.Add("cy", "7836d6b0e5a188f5d2f9fecd33f77e36ee86e4cd4c758db6d2c905b0b12b38300c9f1673667809ac8ad4a4d0265f093bdef22817a141f8d231f85bb7f940ade9");
            result.Add("da", "13e9ebc11fdd591e6834a99bdfb0cbefdf8c7c452d125bc754df58ea26a6e7b8bd9e17f9d657eac56a86d347b9916947fed96799c5e5a650852eefa435443c88");
            result.Add("de", "9eb1892909c0a36cf48735bb4d21859aa261017f374368be4ba66fa1585e4a0dd330f3100344b6641d74d6bfd9836bfff46f52fcbea087c94cae4f84d718ad0d");
            result.Add("dsb", "222fdec279f2118f34ddd048a5921e6e4beb4e3a592e9e8634e868448a4eb63b26536744062d82610f8877d7ebc943f1bf5a02697f61a062ee52ef55e28345ab");
            result.Add("el", "c6b25d563d4eacfba47b0e5bf4bb7e3ff01064b75a83151fbf4967d8f7d050919662b37718cb650a9c733617564e3fb43c61b9f24b19b5e16d115f9979ed256e");
            result.Add("en-GB", "fa002b12e8d78d1f568bc7f333e70be8230d9ba5f4e2aa1c16568b17dde0b0d1cdaab4645e9025f0b540787d6954d9d8b620a1b9edfe76b1b60e3a3f68b69ed2");
            result.Add("en-US", "5176648f3c114390ee4a6c99ef25929303d55959745003f6272a9f39251ec06933623f74da9db1bc11798ba2c8685769e8b7af5d2c123649a13ccf3974fb14ea");
            result.Add("en-ZA", "b00e4c4433438fc9cf169937cc2bc0b4d847e3e95ae87e563791f0661bf5858ab5cf02a72ab19ead78ee665bdaef98e7eabfd8d6aadcbf04967f4f421c480c0f");
            result.Add("eo", "65444b822c39fbc1733f366f61ec4565d4d6e007d2265044087b83a834cb1d71b4bd84cb26c527cb817e6d0d5cdb4f8148d334180c8c9e079c9b5dc8f19db5c9");
            result.Add("es-AR", "3943feadbc22348487d94452d2074a3e741b6fcec52b5e9b0298999b68b94a6ab905fe2a5fe63bcc156f656ca59d3e0c425a69d02aa0c026ee211eb188a40044");
            result.Add("es-CL", "1b0f2a8f2ed9538ea4ed5c8c40f9c380dfeedfc03530e671e62306357d04cfd868d052cc096aab9a6d632bbda637ae13352738c26b7e4a7a007bbcbc18e7d3de");
            result.Add("es-ES", "de62201ff7d6f434cd12add5e7a92d4ad9e8a0344d289f4d84fa89d71f7082ab709441a3e9d24b7bf3f7330cb62e9db2414d0bff57e184fab96fa2f113151f82");
            result.Add("es-MX", "ad74a166aa3f833dec9f75f89ee3c3a5e744c43b09980dba81e1e140fb29934254f2a4516c73c0eca0e6807b76c98d960eb9da11099547441a30c45d3dfc143a");
            result.Add("et", "2fdf82d66876aacb5a0e1d1ffc5eb95c1201306845c9da369b533e46325103348e4490699b26ca7474ac8d137b3e266a47ec57591517c8aa31d986af1deca083");
            result.Add("eu", "990c63c893c4f8a9ca69f6778f9355a5f5b3c9eba8a0c2aac25843ccd5b335824357515dc10e4b9020c6d77805a11a565095868adc086ae46f39861bb7680091");
            result.Add("fa", "1fc6c292eb444d3259514d97c63661350028c5878aee5147b8a75fc5614b5b7d4f8e14385d8c793403e484474f2f7e4a16ffd46af982096025c70fa750cba479");
            result.Add("ff", "b76e9d8633c31652321583fb4765fe82b0cf090a70c940418e21b568902af50ed35f7bc7440dc27839ee2560c465153dce37c5c0ba7ed1492c160d4f494b11a0");
            result.Add("fi", "93c07d531d5d7b3ebde0a3dd0129548d1cd0a6a7cbb1189d3e37081337ccd7a75fc2df94cb3d4f9ac97debcc5c201a5ece914a186ef6385956677d0c58848888");
            result.Add("fr", "0f6ba1f6cc9e4ea9e0fa5fe39d2aa4f2dcca2506580910a2b33ddea97c8566e65384ee80add8eb2d8651d143d1deb747fe006daf0b1b5909ae092896c9c18d4e");
            result.Add("fy-NL", "9ae8076163f0997a94607565313e46b95e5583cfcc6a161161cba20f7cb1a3d3d8d1a577757e323c679e51292e5f1b6af1b9c804b763b7914875d2d94209cdef");
            result.Add("ga-IE", "3e928415e61a886b5ac0a5e73db98bd704c02d7b25cc0495df6a0735117042119d6280e1f68097431fe79d40a8aaec45ad11b23b47814fab243f1440cd2e16c1");
            result.Add("gd", "a44662c4455caf733517978afb5ccf61e664c5cbeff9a8cb9d026198cfebdd88bc21d9efc5c9692efc69883a577f71948c3b37cd44a96602b47032f303401bd6");
            result.Add("gl", "979e8946c7f2e4f34073c2984cec13f9bb92b3f5a7d60c0bae49f37e67f1ed89df6f41d76b167cf6f4a8cc3db2e33e535f9db17a70afe8820993092ae52b6ad9");
            result.Add("gn", "f33d0d059f32034b21ff1646447dd7aed39f81a23568fc9498c3ad31ff7d9dc83d83495991ffa18dfd7124e5152a32c8e92a513af3463b524c9a43ead731b36d");
            result.Add("gu-IN", "5a1bee9b3c738750783b745f092688a1e057c237439abef0c5cee7b705c55309ff04d2b800c35d8d8287bc880ec03d10d05f9cca6e985d4fb70d6bc38f7f2a59");
            result.Add("he", "a640e63ad90da5c84abab8ea5440f18bf1485fd1a0e01a8f53a20314bb608ead749b1a811196db6b0de650c2a36cc33fcf560b0b311345ba5e92a4b9caa0b78c");
            result.Add("hi-IN", "fdfbbe5b2a672d510a9782fde52a08650b751fe7eb2be495f36d66b53107ba9eb881c782e9facecd89cf3e93d566d042713ee028f5b880f91c79ed27405a3320");
            result.Add("hr", "d5f7d6f8f8a52f42e75c5cfefba6c57a0efab4c9b83daa6340bd35b83e1febd252e351f3a70a16662b618f4bbc170d93008ccc52e3203f2cfd1d0ac04bf1791e");
            result.Add("hsb", "e863446a34d551a3893ac2f392d632a2edc1afd2ddec2f144ad2a4ecc66102dc8cd72e470775f55e4b15c925a657ddb24e0f01a5a68556d3744e93fd02563ab2");
            result.Add("hu", "61cf8c69292f79101d1842e4f9a927a9070d18d61b09f0ef41a69b1ed8f970bebb253f801f4555cf2df29582420939909b2ed4d91141603fd88b1b52a9fd1b77");
            result.Add("hy-AM", "7c81913f61c04a66afb3a8448de7108ca067b603e1dbcb380c74137d4a5311a2a653014c7bc953c560d8ef446e3d8368b6f4c08dc0ad36326cc57210eb4ce3ce");
            result.Add("ia", "29de5abc990467c9682e932820db3e0bf29ffe192aa528133fe0f9b55f72f552c3a0e6f999e41b8a7e6ac9ae77eae0648611eca6d93e76ace3ef103ce6adb967");
            result.Add("id", "1f55c769efd7361eaeee40d4958478b8102f74e53c5d18a0f437e22139a7fbdf3f7bb386fe178b5597850067d7fd198189bc1638147b8b92943ce65b5f08a819");
            result.Add("is", "ed6ccab2e463bcad6cecb835b0f920c23e359adad1a6db82a3501ae6e2922d1b6d002c9cf61fe07cc0a52ca488734baba58fc50da00ebf6a17efd86e84d008b9");
            result.Add("it", "75299a1ce4a9a85f0d8b2ba0de176af4dcec90b391194e940539293444b4bed20d05a3e1084e5bb163ca6d22a523b4feb61887c7e95c09b69286f01d3c791908");
            result.Add("ja", "77321fb6a19ddb854a50390dbf781cbe229e159e46e42f0f36d9b347a825dd2fe4c0f94053788616e75a47f3a67bc611cac079da2a618dc69309c991d3494d4b");
            result.Add("ka", "36acf8bd85a0968beb4723a9128f006a82137b87c8fb7190219eceede96fcba46080825b7c046afe28da031640ad8672833f3504514309ba7e9ad195391eab50");
            result.Add("kab", "dc21596a06894b2043de46eb7a6877863e2360ae28053896067ccb2677c1676b9426ebe952216cb5d4746aee60cb7f3cc9fb7a751437bf9e7e8b4a6bee11b4ae");
            result.Add("kk", "8cc8ffaaf714b3f5138dc6ea9a6fd27f4fdf07595bef0ad9a4ab7841d6bf5f137463a0aa8e910fc3d07333f36639f1be5b54efe7c80dee093eb78cedfeea4b19");
            result.Add("km", "18cc994e73b4abd4c6a5e3d2f163c23941092f13cb0691a958540b36aba8d3e4845d05c745ec434907b4da0e7ac079a91021f5beaca68b49d8a5405141e9d499");
            result.Add("kn", "60e6ba024c5ecc4b4d26a4a93254fb56ff2d737406bf54a6f72fa83c51709e3ef9630a9cdd5e9e545fb6a17f8e6947052caa7cb2302b03174ede68a65178a9b6");
            result.Add("ko", "bfc60c6f16c3b92266962a7c8a77a70d1c1dc03771e013cc1bf54c89e249a7755c2ab616400a9a2cd74fc2d5fbcfe8ac5af87bc6a19435b62eb08a6913b8cc8c");
            result.Add("lij", "2df78505e0f2171e26f3f64606dc0b1e233c12dce83c618c9bcb0aa7f6050e1a3c9bfd7574997713513cbc289f74b998095d78485dabe38cfa3a2020a34b2e2c");
            result.Add("lt", "0954e008e95cbf3410cb49e4bbbbbcba119b4b91bd7110efb64afa7cba2762a2d129ccf2cf4d2acacf3a0c164aa18cb070a2502f428e22c32d3fa4a7e247026f");
            result.Add("lv", "a95136630cfdc8576edf1851ab1fa9e6ef9bf29ac4c1adc0880ea775a38003e090a5a67ba4f62a53738ce2a784b530a2e886ae92ffe1e302d1d5519e231676bc");
            result.Add("mai", "d04e51e521133ce0688e4fe73570b9d638532e98a0af7b7987c74350c7b101a056bd9740adf92a9d76bf00cb43d5b74871543212f5b13617baa37c49e4776072");
            result.Add("mk", "dc690140ce3fca8f672eb2fb17c2275ac5d2dfc05bbb0636ffc848065ed603e404ca836a2efc37ce2cfc1db77de2fb891e3fb14d43deb079fdfd374989417ae8");
            result.Add("ml", "fdd97b6b98f50d7a52097f82cc511701d6e9b223d07671a2eb130607fb4e99ab6a7327724d702fd14386675d1a7d70a06d158ec54f434cdf2228b34957cbcd02");
            result.Add("mr", "059440527c8c0f331ccf63d6d91d4d5bf0bf5ac843299ee227b3ea3fbfe75b40c5a764b8a775cd05b6934d25a3fbc91a2dbb7aaf726285b0e989e8eec7d57817");
            result.Add("ms", "42d8c1e919c4434c4a06a1a3f4403f47fc849f144ef1686ae27b1aab6a0e933a0c1d42a7ffde65645a57e00a4c3ce1b8f72e60348898452a90515a7363e05111");
            result.Add("my", "f7eb07d9a45e431a7eb593d573d64e6c925e45ee8f5b382fe57a295c2f3ee00b39c4e90df02fe55b982ea45b18532a5d7142f93f47b754c81d2c7bcbf09e8b12");
            result.Add("nb-NO", "416a80b284c62e6c851c03821bff182d7733a1d65adfa4df74ca48740f380956496b809f5d0af876e22653110e62191de3eb84e6019f6d04a671f909af466ae9");
            result.Add("ne-NP", "b4c05bd2bab4b90b49dc44d6a2b9be7cd17f2dbb9559418ba8679eaaf5f6842acb6226848fff17c388594f8bcbd61aebc5d7fcea74fb73145faff603f1e59a6d");
            result.Add("nl", "932dec6c3897a0f9039a86fe434a7b512b6eea260e93ff1113990c2769e8816d01099332b73155fbc444594f0d6efdffc696b7c19a5b71864c110a12c9c70363");
            result.Add("nn-NO", "dba4cc590f2a65afdd29e17c3cdee2b4ff1a06479eea091c561960a9f2844eb03efa1fbc22fe541e148ad6d25fa2cdea3b70ebaf01b38b31bea458b1105900f7");
            result.Add("oc", "e5b46108fa4d01971f90b7a166f6f690c411c4ffac0c42cb9cb7d0e64d20f9d9f336d4a4442d8ea5793e4a12517d69d197f8280de2c756739433e5e07f143c9b");
            result.Add("or", "e1540c47670aa65550cb68f0f9bb73355a0b6866f19d9e6f6229328e65b8365ffd77d5b458e7848388b6cb25929858bf7a3e80411860a7cdad1e8c2c484f5824");
            result.Add("pa-IN", "8ee32ee3c37da1beb1a9120da2e72bc346f96604b59e4fae09f88bc4e922ed7b42ac9a7547bc68da26683b7c04952b6eebe708c9233970f2b0ac6f7edce76b51");
            result.Add("pl", "b52100657afc02eba55ecf9663e9ac6068c6c3b6c5d04761cdfc94769d58ad2f0479531a180f8d5dea53f5488357b1dc52742851362a4731370405b2b9f0818d");
            result.Add("pt-BR", "ced24fad1c429727da133d7a28a8a90a7d750ed8421f621904a898bee970d1d4161cf5e48a5df2a5dacbe2651b76ea7130315a3d50639fd6ff3221a70079bc31");
            result.Add("pt-PT", "60620eccf76b216a8e06137a71c9c3278c6d88af3d4c37edaa992379152e1a4b9e78b05b6cf24d737497140e70e43eb89d40135fb95d6907a9b60b7ac5045e66");
            result.Add("rm", "5dedb4a6f1ec2c0261343c13f8ff08cd009434af2d1dbf617666d9990b910c1ce9a9c1966b3cfde15dae99701bdffddc6efb39d5454152ab0fd78f4543a3fe2c");
            result.Add("ro", "68a9b09245f6da0a1bdc3f93e05028017714cfd04ff7c922e769afe0efb53987986a9d59285f05dd512e69dab5d666902e74d645e1eccf9d25a8a2fca062c5d7");
            result.Add("ru", "41aa739252d4aa98766c7679d0f2d39ff9716130c13b94c465c8d37303f95ba97d7cd848fa2a9a2352acb5743a2ff88836afc9de1b85c2e44346b9074af19915");
            result.Add("si", "71ab0dd777395682cb3c0d0e44f3d39892b3db8015a395b336bbc3e15236943fe3c0116ec533911fd1b9793e8a6821af298488711e3216f8e077526644353b87");
            result.Add("sk", "1589ff49551aea15f8d1778154a4102dc18651317af93b5f00365bdeaac00226b9be6b2136898c8a3e8b0ace1417141c25bfff8269673133c1974117301d171c");
            result.Add("sl", "f212ac0c93cb23db55d6a0f7e63a0dbe0a5aa756728a67d88fd969d141d55e393156076b74e7bb6bdb2bd7a22afc28a592c5d2993dc9c2fcd06f3aa84fba5e0a");
            result.Add("son", "b3764dfabe0d963f2c506ac7858137458a8bbfda119f85621702f0b32d678a251e2f672cd70f53ca486e058204eaf3af6fbe769588249a5edf5e2b594e75b7a0");
            result.Add("sq", "f671cea41eb61133ba39492648fdcd68b04faba8a3a4874516e216fded8bdd63961fe2b1478362ec228d386fd3a392ae4e1e8a72831d9d5e8dbd9681e743fe2f");
            result.Add("sr", "d25ee0dbc447d215ecbc9070383df47ae497bd5e39014b46971a5ca1bc72f2c2fb417541f03466459bc826ab49dff8579632090d5cc18111496beaea2f59b652");
            result.Add("sv-SE", "320589ac80de6fc59f921a49cd270e10e96fd3bff6ab54b3d9f6266758178a9b4df64e510a2c40373511844630c268cbe6b38624500defb4ee47c3a5ab64acca");
            result.Add("ta", "2b60da4e23d594ceefd6da2c4a5105726c65b14abdb69d5b2325f924e21c1d914afdfcc637329f95b2cc7089652937319f376289244a3a823ff6e96249edc24f");
            result.Add("te", "f632b7723b36fb8da556c8b8c10e8aa0e6c82060b6359c7b3b178450d1c1ae4c0c06b4f2c25d2fae3ab93719417816fa775104a2e58fb75ed2c641f8ec691c64");
            result.Add("th", "0e04c98a661aa1e80deb51c2cbc3a5f63ed81b4174326c8b2ed11c94c0f45687c33e12c1c4d6d80a42f2f89f72a222230cf8a862b9b90d860c0dd50339059e9f");
            result.Add("tr", "0f6706f81ae06e9a3fa4a507b7ceb29a2823310ac82077bd59f2176e24c093544e93f32ffb4f0120a55e09efdfcd405c7fa9323108df998bedf0da20c4d9bcf0");
            result.Add("uk", "51911e65563342acf36e2f0d639734c1df49e5848ec0636a2b9ef39f952c10b5d90390d75bb03cf599eb9e7610aacc40a8b56e343fd75a6687cc49fb3be5b8fb");
            result.Add("ur", "9526820f1dc6bcf3216a09883c14457b7370aeb4f82a16076e8a291b3b2065a925eada32bac9b6535e1fedf2e8c4373a3407321b5c0faa6a36e752e3fe860cb5");
            result.Add("uz", "fbfde35c3e54fc4ee7574758254f4a61507499e3fe9b62935444a60984b6ed82a80cd99071d28e57a0eb09ec8a2b8b23e0aadbccafbee39576ce35111fc4b174");
            result.Add("vi", "ca4d2a11189427f905fb92459596ab7d995fb81e1e832f971bb0a082be2188c853539933ea7ba7dbc46095acfef98eee15245b3876d1664929c59f9e0fd30699");
            result.Add("xh", "a396dddca0b6a021f1ac2e4ae0186b8dc0467666ef816e9937daf25965d14c27b50936c01af1e4c727a1a066f9183f783dd63043dcf8d9bc9a489178d5ddcf48");
            result.Add("zh-CN", "62e01a0f3e5472ebbb798214fdc7a4e81c0d1513d7c27160ce34b59dc895c78b1c3a6ff8d7969c2f862ae51e1d1a2c86eef182269495c8907ba8667ab505fa11");
            result.Add("zh-TW", "6e6a002089c249893e938b96fd110880f9dab31c0ea2dbc8c8fb2ff7dc14d6845c30c6c6d1cedb84de63d0aba2bd839768b05b6285300d9a6131750f836ab283");

            return result;
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "60.9.0";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    publisherX509,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    publisherX509,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return new List<string>();
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
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
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
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } // class
} // namespace

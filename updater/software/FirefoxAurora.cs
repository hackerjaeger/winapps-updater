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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "96.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/96.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "950fa64179d49686a83184cea1da397b814c6c7e7e488ee97aaf9c62d9534839bcc3cfe99fc0863fc7d54983dd19c76ebf2846d58c79a1169a7f78d943a83057" },
                { "af", "69f7e58044de7d3be9fd73f128ebb17be11532ce2e1c781f5d0137a505478dfa98e19fd96ec90aa06e3f1153c3ddeb13848d0b310c49de2f09d1347c08c5d650" },
                { "an", "e066894e76331500b65fd87db388b74c7f618a23283f66759c9b447ce71d05de3397a41d9cf568c9cd45b4e5ed3f50f125aa8ee165b7fda581df59bb22b2063f" },
                { "ar", "06e316220c87ef654ce1dcb9d8b0222ee56ce64a1a3455c9a460951a8dbbd7ecbc2830904d17d1a65b56ede33a5d9ca9d2991fe450298d42d0a97ff80cb4495d" },
                { "ast", "9e53171dba57801fceed60aee291cad2f537b3b62fce1f590a92418eb86d4e9c4a1d9fb995a585181d45c8dc2010d01cbc011ff8c9dbda3cbebaf442af32b4fd" },
                { "az", "f2719e45943c34c9eda6db92936f344e6a67722bc98feeaffd3a72021e1211e6c98f91727304c14da276eccc7763f2ae116c4f9b6dd773d0ee982b45319d827b" },
                { "be", "b44736ae949cd6fbfdc75032f59b6f703e09d1acaed310c83491e0047450437ef12404c55a36639edf52cda47d5c1fec2834db514bec742fa0d382062fbe284f" },
                { "bg", "6726e571b1f1fa42e14204acbff67fb85eaff5bafbfbe370f2f9565877104a0c233ef08e84c7d744d5c4496e27953b8682899dc2eb7751c1f291c2b2734406fc" },
                { "bn", "0dd900a790087201d3cd08beae516e7ce18cd930fe7344f8ddfa8e7959e8e16d24c15be24b6b4c1b010d8a0b680c3ee2d878738d4090f73c325d6a487343531f" },
                { "br", "e531cb2d9dd48769be072a58019ecfc523c52237142b3f8ceded9f97e156a7f0d1d3296dcff12370782005f69ee2967888b0774b7b723e9e10e1deedad56b70c" },
                { "bs", "e3c73ef20acc7bff807f13b75810744890523ca67f7a4fec9f94db86e97949f55609ad7df27533366a992c307263934c3e8619390e6ae2840f74dab135e284a8" },
                { "ca", "b0aefda41fde9332a41e7854c0f36099879cf9c83386f5c43bcc0b28e11272f699bcb13b67ecc71bf90d3add8e74809139dcc67865126b1e62cfb2443d763302" },
                { "cak", "2055f0de5d23fd5b612468107f654a4cda39da127ca30587dfd87bd7999da9992151e383939731d3552b6a9c8a20fd352350c40f14ef00eb600def5335e01b8a" },
                { "cs", "10f12794f2a1767bcd6969df89dbfc10e0bda89cfabbd355914aee2e81b7a99b10aade9d85b2c703eda7ce50330d5f334932673150cddadccf911ffd2c39e51c" },
                { "cy", "e1e3c85215142cca9bcbd079dab3094e3905ad5fcbf36c3d4b8ec869ce3afcf189898607d5d6c63fd759640965d3842b55a008e8a7d57c1ad1711df1c1ffc37f" },
                { "da", "f02b8271fd592d2acfdffccb003e696a3d4f804fc1ed6e07336a46f84df78dbae7b3fea0714767352bd1a94de96772afe723db15fadc05450026b36e7ccfbf4d" },
                { "de", "027de16b6cdecc47be9d4c53ed07e56ecbd5877e5a9133bfb4efb56bf1572b58c2945a66925235545d5ebaaed22c8c61f987a59d7826262442d089dad5939f16" },
                { "dsb", "c90fc96b32d17c925f0be7514230ce2c9af42fbec9ffb45afa962a9f6efa7ce5f9dbb31f8bd294f3daab70755b7eb91e5ee8f9fecd426348e1ee2766e670ee4d" },
                { "el", "f6f80b6af0317de810770033749f0dc11dc9111595b4e555f7dc83764d55b297f47903db86806a935f5aae6353f157f084bc046a6fa05408d2b6b5548d236cc7" },
                { "en-CA", "8bce46e3cb8316b3d74609360befb426fa3967fac3930cd887fbffa58c00aaa485e0f0cdfd75eb3f2b0762f16438041537da42252460c6b87a0a83b8468852b5" },
                { "en-GB", "368f8bd90fcd31a462d065c336358d722512cec8d1b861971816d1ce010802cf6889efe815388a9ed3f609e34b50794dbb4b2ffad2cda353523cea019bc4b66a" },
                { "en-US", "6b6cede9a9eb1a5f8f9e1d43a2be9f74fb27c03ea1007d28d16ca5b836413cb5b318f95e28aace7874683d237ade264c68df9592df8b4a442d55907020e75341" },
                { "eo", "c7f213caec223400eeedb1471f1556215317298ba820ed264239b914cf8f7431e607cc520ae95d24e956c0966b7c96a611971ae6e10cd0c3b8619d5d8308f6f4" },
                { "es-AR", "61d7ff5a75ac27d0c93464e28cc06d726fb51365e56bec25e23619f5558a717dc4ad0b4940feef72dfd0b18da9a93d1a5251d7922caeb03adda75beb022a984e" },
                { "es-CL", "9c1b0a3d71f3199df264ce49595b34c8213b113fd43cda2bb67308879fb813244d0dc5b0e5d71201126518d2b9d5815b297a7983f6e7a27e3af125f10e2f093b" },
                { "es-ES", "0fb2ebeb3ae76fa9ea4075c534b857c443996d9edea47d9ea3fe00379ba736d2c830b37b46de4eb675aed749f591e2fe36563a81dcc2b5b3c1868c87763cdf57" },
                { "es-MX", "bbd5005dbd1dcdf93486bfb9e8e74eec7669430e4f20db8360e7ca39133150cfaa2d5dab4eecc3423efdde7f1d864c374cbe9d2ef29d75d81abf271b27f9fd75" },
                { "et", "0799590122b5779369d4c5eeecd3104aaa503556d6e17ce73c81b3fe4179014b6014c56ecb0388eeb3aa27f8d52477079bff9e3c48bede0c40b998cd41fe2a6f" },
                { "eu", "34bd9e1db35dfd02db44ab013fcbb9b390843eded42d9ad97584921e76de1a61dc6b076c9c1053f2355c864bc3d2e7f604e27f8b4997ba45a107c3b3b308eea8" },
                { "fa", "1f49447de3211fe06c1e4d3523dad0e238fdcc5eb7e57bd43f05f2683deb51754726b970528d6a7af094b3af340a8ea80d7bdad932d2f0ad467aab35f0ce9827" },
                { "ff", "28daf7b151d1a68a7dd18ecdf67f027c2afe5b5de1e448bcdac47153dd8b549ed95b2f7c01a86b7e7dae0bc29cf878947178e8c6c62ec923b8040cea2e73c313" },
                { "fi", "af564e680c456aaf9c8460dbbd769681c40bb6af7116a01b9153b6da7ad18e5d57ae647208f6bb4d4f724a7e58d5ead137303fca65ccbb4d624f045034fc8aa8" },
                { "fr", "e8fabb59c4951c4ac2ed0f248546dbe135a1fa3af87cc8f9196da6fcfebd4e8062ac57a42f31d13021c83833d443914a7bd1f92c62d0a140dce6ef1e23ee7878" },
                { "fy-NL", "3a6ae0d2f00ab1aa9f0880538406922f6615ee42c0325c43a10f9c24515875a0f2d095ecda331b022bc22b625057015d34183c1280403526f0664f510729d653" },
                { "ga-IE", "fa02d3b4c09788d072d72c03aade6bb68c9ccbfe7702824c10c0117b104c55e0f9482b2001db9e9a94bcbe4d29c7aa45c65b0ba2ad0f65206d422547672c7adb" },
                { "gd", "274892f5ecf4509583b72f7fb97eb8b8f58190f39702938e2caab5dfcd3eb5faff991fd92b5621702b6dc1cc7ffe97687e65fe61f0f1bf3f198ad617bbb52fc6" },
                { "gl", "536997af97c4e0321338a27cc60a00980756b76f276c18ef1b5cb4c009c98a70abd30ea8fe99f9f52a64e7e4262688ef30f5f42dc8d219041953053444ce042b" },
                { "gn", "cdc0cfb56b04b01885a09728279eb84e2c98d3bb80b9124985bbdf703fd51b9a00ba10e4766c88b1a00c928acd6b0a7f3191157b56bae52ca01a87ec3019d7f9" },
                { "gu-IN", "fe9d9d82d1cf9c9d501742a2cb8dfd85dc4944302f009289a7768e86365011d57fc20de51deac8c239a40bd91818a64bb74154f0849969dfba7ad62673fcd21f" },
                { "he", "cb0ae2bc320095ce0e383296c5010e42bad1d07e74b4069221ac20d7432d5c76500ece210a6dca8b6b953904187f8f00e24e0f0d5e9e75af5fb3ec4cce29e29e" },
                { "hi-IN", "100db527901c3f82829511815a379f04852153fede7f8706acf20e3c050f32845558d5e0f43523b7ed7371be0621c187e0bc81ac5d64d30b4a6ab0d769791b20" },
                { "hr", "36e40f58806494347256eab1f8f69108e4402ba0db9f5f9fccac6c87cc1602c36ce1ea52a4a4e036fd63717e7529deb2405959b933f836c84e581cb6eda97c5f" },
                { "hsb", "b386ceaef733e8566f54d387f4d8c561b7cf57ae32c850b9228ad290b09cff29df5c4c7525c20a8ce5f63819599cbbaa1a678f8cd449ca882e48b734e0d1f59f" },
                { "hu", "9745893f366592c76ee191b0f3df0183a991881f481899a85252f29ac3b363dc18fc15a4b6b2aa192640620cf4a2af39a2ba0f6add27ac95cdf4633f31777468" },
                { "hy-AM", "b779c44630b8e5d4646ce0f56460dcb92838163bc74b37b9054e6c67abcc0f69550ecea61fe6ceeae8b0d8a2d16fca814e596220d41484d05ffec27b3bc07e4f" },
                { "ia", "415f36141840b888e99d3b6c845fd8389dd807a3674716ad513d7777af0438e1eca3e7080f9e802b82794abe5f8694f985c2c8ebf8a5e35a18f0d6208ce5c57e" },
                { "id", "b96b744e967184d3f30df1dd350d91575d683845b6eb284f32c860304b738eeee2ca01601fc8019b3e2df7369016713bb1b0a199b54b9a088fb128ec6e1c33ef" },
                { "is", "110e8ab6bc88213446ec57f2ec031184226299a7aee5976a7d3dbfb76bf933203a8be8f0b3f665c0daf18fc0b745d74344989d0915790dfbab7e5fcaa5b9db89" },
                { "it", "a9ed60f851a41134aa5d514ec72ea15428df22d498385848b24d914926362238049d8f1d27408baf2cbd273600ea3ff277d64c24942812fdac85a69f4d261d47" },
                { "ja", "baf96020ca9e303a359c3d388e4947bfd25a01dcb290fc3d7e1013e28d5dfa2c502ec714311ffbe0d268b3c4b8daba6cc9c7211a42026609c383db9ba5610507" },
                { "ka", "180cef853140b933f01a943d060e925bf8a37ad71286caa390dbdf124a96ba4144eee6a0bcbbcb0d8d654c82156eff57d43f5c5fd0b52cd1458646c5026a3c18" },
                { "kab", "bdddcb7e21c54cb3d3b52207bce47656f6ccf923b34a1af8697da045f85f9efa543528ecb7acce92b355e8d1f23ead3ba4f520990a6d212d01b829ee04da43c7" },
                { "kk", "a09d60c72bc776e1f46fdc225c4e1eaffb0fb8a96db759e21158eb9c8474b0811ea8307e2d5d9b566cdc23adc55af64bf98a10643b2d327ced01acd8031489cc" },
                { "km", "1321ff45fbe582154ee6e50204d5f167b403a158f53790df358e3518754cd264156dad39faaf51764e4d07284c8b729884d7fc30e8e6164cd8c080db638eea8b" },
                { "kn", "8c8c4464d862942a3f31cd81ef7301350bb613e112a63aee0efa2b9ae0f6396cebf0c2e97d67bcdf14e57eeeb14002edcd5d7c60c289a84cc4725100cab48b89" },
                { "ko", "401fc8b3dffbedd2239dd7121007389caf384c086ac5d6b6071226e7a343f77bc9ccc3c7b949c1a4d36802c1a7e4599347a486b9b876c8a99bde5a151f31cf94" },
                { "lij", "b225e56ac35b74ea49ebc83eacbb7c713785d48c921a27816873f39c98f8c64d9f170cef1873d956aa8891a953d9d5f79c2704128ee07ae5c4984e6b02f524ac" },
                { "lt", "443ff8efe46b85a8eb05a6c772e37a6b08823c2d80f3bf9213b66074b7ce5c60ba944c644d5f8c2884c08bc0cc6af0dc7df7d72a22d20b019b074827d979b8fe" },
                { "lv", "e3cc59ccad60e6209a53b53709f3c36f271624ffa1645ac72cb955fe14d609c09e90e5cf7325a4097a12237486c9e2f7fb2a03d6c939f69b4519a145e3e925b6" },
                { "mk", "dcc2723e44ffe1b73c0d869c73614d5367da81a7ce5f0f5704b6fae241a22df2c7a6acd83db92fbbbb323a25e22edb35520278366fd015229803f5db18b74d14" },
                { "mr", "7444a23e636cac39d907165927538f42e8415a5d3c50248d801f01a27f2337ed8cdd512df18d9af38e68ad8d9deebc59bc236dd1001e23e5605158f33c943d53" },
                { "ms", "559f64e1a3ac2ec7231a5e0d831e12273eaaee965ce69e9552803c0b640b35e0252348f5fcd441f5b4eecebb1cd8d8be20a4522d3583701607734690f002cb45" },
                { "my", "597234510d1e3931488c73411afd59c43ecb901622234b95d6bc8d4ca3cb2045edcea9a8dcae3e8286e610301836272d6409dd9024fd3f6b065dd17abc7b6c8d" },
                { "nb-NO", "75b6529b461da4df9fa0c27b5d71419b3be6101dec5c30496bf0d8f5392b668d823106e12736e99962f932c6d42f0f17adf5a786cecc8c186a0cf753a6e16f58" },
                { "ne-NP", "6f5a32331d2587aed5492189fc02bef91e085b7affc42c6b329d1a0c43004dda7ba1347849467ee15aad38ec82498ab02c4c8566360d2ad87bb4e38a6380bfb4" },
                { "nl", "4fca7c8048bfa5fb7727b5b0a0498ecc6180f8a204e2fbe0a3c4619cc5d7cc2f975e431a3319bfc3e8fe1874f42725f93112550d99ed7095612069fae12a9c1a" },
                { "nn-NO", "8984589fa25c02d34321af6144632ea0640ff68ab2078873e1af2a021afb85bd50689d254d08ddb025b2741d507a8f8c56dbf9c203dc6dda1f0fe264cf433dd6" },
                { "oc", "ae80148a18be7cd3c6baf4d8cf3c9d574e4a11d469d8f4d27a8fe0fd2149b0bc4bac9d8e4d444a81ae524803456f1811eb6b7ad45f419b5c5a4b7a247d199efc" },
                { "pa-IN", "aa6110754037e5ecc9c164bb945dbb3c420ad23ca3092499408ea1672b848386924499988013a4abc432f49f64bf3ac6ebf20dc703f6373cc4a879e9febdb3c4" },
                { "pl", "289dd902e4d15678066f1cb343747b7fce3a2d5e20a50f5b9142f34777a3280f058399b5b18cf1ef3f63e6aa384653002d39b9ef4cfbe559c37392255d0b3569" },
                { "pt-BR", "5306094b1a283b256f587c845d204732340079dffdbed76ec960e695e131874be4a0d2c665ef858da28fed6ba78eba07249e8cfe1f6790c0e1d01ac06c4db48e" },
                { "pt-PT", "b4323155debb7b8ecf23a715f511369422e75773f02b3207c6670c2be681dffd1f57a9f60d455d6a6bc0d33ef201aab7fb005c1cacd7d74810835be3b295f44f" },
                { "rm", "d8631a7cd8e2371adc011f341842113534c3a281330133b7e30477dfe9276f67ae516e0640b0a79520796ce58f070fe5f0e3a3c05aa244ca7ef6e5c345467790" },
                { "ro", "d35587d368ad417bc3975d3894e54f3b08eefd59f513bc64baa3396e2ae2bb6d024556c87694061b7e387180510d2240c4dba7661fc7f13b07476c3a2f583ab5" },
                { "ru", "7dd2376bd56c712e283ba6bc8bdfef2d496058c021e903519dc069d2d3922212b08752a2c33d05f98affa8260523474379631049553a04308cc169155128f37c" },
                { "sco", "832423b9dc85b12d71ed2474f4a5a5bb918167c8d38d3b2e98dcebcb980ddd95ecfedf790de99b473fb423ac30b43ad885b6c52775510bdceed0d7ad34675781" },
                { "si", "f1709389c4670f27fa6e940bef46fc54b2f8c61e078995ac05e445c45f3aad68d68e26400ae5ea0fb030af36510bb4dc4fad424458e4d59c76d098968628a877" },
                { "sk", "08d15179a2d8406a482bece0d55156fcdff45d8714dcb9077c0b6e81c25eb9b2d1bdd8d03ca3ccef4ebf1fdcb49b7629c85c3f5a368bdab3789b925e42a30caa" },
                { "sl", "482848376514677d640c9f4d0e139c9f51bb6f122ae36df91aee365e4256cc2d469453716d212dfd22d680e7b6b76aa4257b7f750190bd55a4f6cce277225a2a" },
                { "son", "79ac1f524a38fd61979509bf433249107655410c5acf9fa08a024544e43516ec90cb834f52a65ca7b7df1dab4aac90bacd6343c1ceefe8b6041d13b47fa90aa4" },
                { "sq", "1e39e009e825b6f74a20d7d57a593504896ded33930242325d9ef24d95053771e52237414fab0f933577bbcd298ec8c06cfd87bc11e746635e102b7f5d793cb5" },
                { "sr", "905b8b2b5f616d7413d0d153f55b10310027c6a3f1c0fa817cbacf8935ceda6bed76f10b70dc263924a3769883c40007e5c6ccf8a01690c75599874deb7976e9" },
                { "sv-SE", "3983121078202d4aaddcf9c240fe277f036fe52ce4f96175f9d4577e1fff85175a0fc998305977fd35cd0dea29d527c1f0f462dcc06353dfb0163320343b3e5a" },
                { "szl", "f850c59f3f76356b696a353f143a8497d7e84384858aca6a93b73e2692350c7aba2d3fec95d6f2f0e75a22c500490480314fc0ae989d3aaaeda3271fed46eea2" },
                { "ta", "cd7942070c519249ed150d21548d316bb4b2e1e7a1a98731e4ab2e1e3f4908aa85ee84a905c17eb9009092e6758e50896c71c3deb567cad52832b97be323e52a" },
                { "te", "8e6b77e47c74c9dcecaa676e431fa222af97f2e82cbdfe533d13cd162ffd8d49424c0c532ef0e61b11c9291b15b08238ab3bfb861e704e40416a117d1c206a97" },
                { "th", "610886dcf4da4426376b66fbc73cb0a1910f0f1e97756c353aa3c3ce1eb6c02cbaf1942dd2d8a320da6b7c023178e6c4942029118818cd93d4a2a3889ee23383" },
                { "tl", "67fb7f6f63d3e0c41565f8eccfb97bcd20ca1bd36b3f85ab627a8c13ef71cb3bae1932479c602066121f4f1b16b82d9da43d5fc9fd17375f51d5cc18afdaa328" },
                { "tr", "7af431989d25f590dae488e4aba5381926913d896779c44c2ad517cfcdd74a529eda2ffd32fbfb985834b029b50eeb015e694e2a8bde774d18c2ab8fe7a043f9" },
                { "trs", "fb1ac58e4429129b3ada563b70bbc4c93b2c4919b6c4c7ed4b9c5677fea9cdec71583ec827a4347b9209441fbb65faea7c7c79531bf339c21b5ac86d6200ed8b" },
                { "uk", "e6b7f5979a6671fbf7a3d9e022786467d052f8283b00be11aea92b199026a835f9f590479b63fc051011186e662c6dc4b324280177df84ecaaf3ad02f4ee4e0a" },
                { "ur", "2dc49af70f1075f4cfe71a9e4527b39a3ca81c9bf3b3248211c49c15b58eb11ad9ec653d8301f387050ec9a4561f145c7cd61f5b76444dea068d3b6a00779816" },
                { "uz", "4769935d520e258b00706a1c6252d8b0acc287893a0805d18c172e39b871666c1d3eeec9a3c3861acef5e46a702df1f745ab8c9c9c5f12c5f517518eba2f1f64" },
                { "vi", "3db1cb1d10cbaa30ab7961bc386ba1304b2c8d49499c975045e714be1b5dceafca0bcfe45e6a355a6c0ca21b80ff56b290691ce73385e5a963cfc10273cbb846" },
                { "xh", "0dac51c82c17c99fd00fc6a805400d5ba572708c6a9ca0e4c45d0d478649618e2fba142943f59ba03bff38a3c6a4543e07eb74609e9424c3b031cb88f83a8b5f" },
                { "zh-CN", "1b45dc1a901a16e93f58ff21cc119220e248f077ea875579989e1e07ea32248489b177ac3cd239b3496b08f39c1b84e8147978dcb897d7abc02ffd5ba79455f7" },
                { "zh-TW", "98b6b36676c631a3189c1840c66e61058d74fd15b445fe6afd391f6e70d8f0495d8ed7120780f411b5f8826ed26396c549aa2ce2de08ee234300e36631f3ab86" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/96.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "b5b83bf00441328b5e8742d1172f602a316af6982806315e1d75d2af9924bb5b88b5389bcad821f88317b5e9053a7b93c82001f239981009cddb5122733afcbf" },
                { "af", "8882fad1147aa26469dc8c62fab40d6ca445880150a650b9d99581f4698cd3539be9b198fb3da26531a6351aae7e1a8e7ad30219241cbb03e3ac85cd19c97abd" },
                { "an", "85b1833b64b5e7a1babc3b2307a87a5193a48d10ff0765e50eb6441568afe4467c4dbefb02515858b16c4c4bfc05814f6f4165befc63ae5d79ed5c21fb586284" },
                { "ar", "0b7039c0a76bfbe2dfa531deaaed943578dc904d7b82c0356b4a1feb8a7e0b0725edbf0b676683f3e1bb2a89911be321d910cee6129bd353265fd9d4038c1fa9" },
                { "ast", "5c81d9893daf85bf80be439ccfcbd1b87f79e218c21b25c9e01ba658e5a8fdbe169cfed7283969a2407c51357c74042982e3930397a81fda4b39a90935c11fa8" },
                { "az", "569562acc1b62d3200868031d8b5b5f23953f7984db5725f778cf4fd13b4b36e4223fb8369e7ebbed76fa78f78b01753d5bb88f81471be24384ac1e7741be36f" },
                { "be", "011763ec27fa54efe4615d0c06ce0962a324a2208ddb9187a6e820e208fa740eee61eb265db59924325b9736969e5ee4938d615d8d852d00b717ce9cc5ba43a8" },
                { "bg", "dba98509f4fe3dbaabe0751cc83aee2bbce5176fce3072688e008b7a753a554c67eac94cbd243f860fd52b9aa624defc3e91025fe6af679baa84fe21c68e0013" },
                { "bn", "02966ec26ca3a49ef999cdd8afadf9b07145e4edf0b1481a3f4507aee34aab7ac8bba13443fe8493ce8e65dd96c63e9c00690f5399b3ece82e8d11d30c88951a" },
                { "br", "af09f9b1094a4ee196bee59ac10a73a2912c0c9c848a53c18e09a43627864b408ca95212f8d6d16af32852928e3c553e490771579ca14a20361f760f27b9029f" },
                { "bs", "2378f674c77dd58c2f9a63bb2a0cf231a1eeb075aec024946f2d20fc90843d17f0290c0743b7f50069a99ffaf68591b7839262fe7d5a99ad7fc707d1ef1e39fe" },
                { "ca", "5cedf8fe429181e49a8345658535343d4f47de04fecadfe853949178dda4f91cdd30bedfa302cbd429cbe8a18df2382f8458f29906854446669be8627b8d73d8" },
                { "cak", "ad70d8c47803cae79074d920d70b5ddd214c8e5322807b310b1459c3ec1859a7487b8be24d0f9dbf71d431d59782e2974b3350c92be2feb534713cd3160986df" },
                { "cs", "e59d9fd394d35e56e930107dd05b10d622157861111343fb219e75fd7a0f2b079e3b00fc1eb30c451a131a3f737925b04d78491f5acc316df337ab8548f4e6d5" },
                { "cy", "d7384064ebb9224b49cdecc64626f84eed5e37fc6f97030c47d9b70d7e25b02feb943800122526cf08266c6e74ba6edfde22123ad47e08c26dfe4838abf1164f" },
                { "da", "e21a222933bb9531826694a7f087ecd4438ba0105be99e62da1bc5bcc002e04093f06ef2bc7b93366937a6e336f63afbba8c57cafb0e161043f3dd9a67b4419a" },
                { "de", "f7f5baef8b51d4c9e2077c399249862bc81d19d2e7bc7ed4bb104becae7fca2d1ca46b5c971743da477835b93cc56db8da5ba623732aee44f64b6621608993c2" },
                { "dsb", "05199a67b850c307469f9a232bef2f1a28420a17d1dfeed07dea2d7ed15d2ea4b01e5005ce055bfee5be2be3654537776520d63f7631a3531a8b536724671db8" },
                { "el", "eec4ff96b5d546ccd1755163949603bd55a54b230a9ebe39bcc5f5e398f14f1a3b439067fdbffb91fe55247e9a0468ab5bfdee7d9bf3ae45a997ad72df872bff" },
                { "en-CA", "e1bcfbe5497f1ffcc6782e7a885588f791539720e2fb9eb1aa003adef377f0a0cefd1ad09cc5b6d41c607bfbcd2f8d70e06a8143dbeacf5239d86a0bbe6330ae" },
                { "en-GB", "a04958be20add56ce605bd0c4c2fddb552eac4ad1f9bd6ba4b0f7606236c2a8b1f0a3ac35c707747c0e691e42d19fc917d13b3f301f07997b585b15c6255db72" },
                { "en-US", "a71d70c4315de784c67b30a587cfcf2a93d1af07a777c1e9347529a0a10285230c0a7239911854207e1071318fa80609c18a61c53ec75de59604737d1aeb112d" },
                { "eo", "1cadf6f5799788208615ed8686d29759bd01f362f634b8b7a2f60c0d43b119645da9b0adab73a09cf4acf391d56b69cfaf6eb251df99e2f4754cabb403755b58" },
                { "es-AR", "fb70dfea6945325c93974db5ddfac34a8cc90282e1437a643a9cf949d4bb67b5a83a4a9ae7142a51da6e1aa30e69f67573e279c580297fff5017f9ace33160f5" },
                { "es-CL", "7f53e983d272c8b7ea2385da686f19acdae65b6693b58cc26508e9677221143227c38865c4ece44d5051a287c27fed101ab5cd8a2eef8462e6db097b0a34c474" },
                { "es-ES", "64273c7adf0125e731f948218f94ce4196a4708c484b4f9031d63435551241c19d77676e4d37789f1c8602ebe69ad6c6398a850e444d230e96ad02f8fc6bf812" },
                { "es-MX", "90ea752c2550a5a0e41129b9ba2140e8ef03e531f17b7a2068aeb28912046a8b41c065bf8ae76c6dcd6af001b1fa9321602833201caf2a8289b6e81b401bce4d" },
                { "et", "19feca6aaea1742007e25d077463d34ccebb166fc2191a5970867067d442777bfb0920a6e0afc5eee571b2425507b11a2fc2fd5acd7286c9e967d96b1541a79f" },
                { "eu", "06572be80728aabf5455105053401646f5c2748d9c4703ae1403c8d6a5013cc48b512c8815366bab570f7df5768bbfdbfc388210cc3b315ad34fb603e7f0da17" },
                { "fa", "07568636818e77be24cc0dd0d66a8e84838629ef75b0bb003efdcf9a9c6b5a85b1fd3550b94d85cbee03ac58f2f8af224cdc372fa9e968720329aa3294adae35" },
                { "ff", "9f192fcc1ebe772808e7af6bf3810d49a9c9d8db7baa5517fe24f60d067b5a924408d9a2872974f22d3549776898f97debd84a5058b85d13cf12e7120dee0a21" },
                { "fi", "3f2fe8115c7a7f162679872be315b0daa0ffb8bbd04b165190aac1eaebfae1bd6ced0f7e69a2b9e30fab818f5d19c09d07ec08200ff4d3bcc8db9d5318ba04cf" },
                { "fr", "321d5b3193fc098531f18ffddde9923497ea707245b2fd3c85115152155fd003e47bb0a20a4a7b17c8ca7b638f781e090983d01e9db00aef9a32a5649fff4e7d" },
                { "fy-NL", "f064f466919531a2f0b5a03d087f79c216d69a3602bf637d54b2a7a455ae59880e9c88da0e3fa7d92babe7030d054d5c7a32c63b9b13304ce098e540143af6bd" },
                { "ga-IE", "71502de4c4ba55fec39b63ce084599ee34c2b6eaa3095c6f4609cf71bcc56d89ddf61d511222088383f45fbd18124b7a61aff54ee5b379adf74d542e8d8716b8" },
                { "gd", "ead1c1caf3de1ab69668d132eab54bf256ec21fa344c88b7b9fa50d5581df50e695b3da06d7e0277ab49c3edce20e1d81d5a4e0510f77aafa28ef4dd589d739d" },
                { "gl", "0b0dd7d5b559a94054bd7c6c6eefd4f23fc230e2d4e96614e86dafbce2b005784e2978b3e18776316cec4b2cc287d5c74c81baae0f72e277326e2aa088c0cae4" },
                { "gn", "04087f964b1e00aa196e462d01dcf3e3cd68b309fbb4d8eabb55345da17e85c896df95d0b57a874c983c9b633e8e86744d3c674aa42a8c95ed99e321a5289e67" },
                { "gu-IN", "a64f2b1be956cbbd672ffcd38b2c9bf172b9582068fc45b8eb3bbe8ac216a96a205978b0157a0b654b330ad1631f8ff8307821392bb25ba0ba6dbef4614d00a2" },
                { "he", "b11c76a2584b54aafeb1fbd49b7a116ea570cfffb126aea7912991a5613f08ee2c2eab2bda27ec4efab8604a1d2603e8308cb13a62d9184033bcb0fba1c54288" },
                { "hi-IN", "18d35994fa96f50e697ed2e0e75738e4a3893154126f171ea5f09aebee4e55219017aa5e89ec9d113cdf92b51d168103954e66ddca6c47ca54e3d1887ea0f628" },
                { "hr", "d527c103bc9a01cb56c085a3eb1479e0927f9f51abd0655c6d51505479651b89c914e75aabab6a444e2aec5bc9ffba6452a004261c30b7268846967b46f82676" },
                { "hsb", "250e01f3aabdeace0b2894fbb35b74a0bc193aaa990d6a01003dd64a15649a744409e179fe4d1fe1a0f86646208e4716ec142d92ae23083cdfcda12c3c46ce68" },
                { "hu", "c43ebe93728a51c0452c810ac4f054e0b47dedab40ee808794ca2614fa44d832ddacf0509fe5c849fdbd6e8d0a81501f7d6fbcffc66685706d00b89690677e1b" },
                { "hy-AM", "a73516340bee7681d654b8d27403fb182bd057212e89632fac0b2dacdf27bfc5c01ce88be4d1bdfb0a4421047bb3671f2f32a07e7675217d8e62b41b4c6d8064" },
                { "ia", "34602c32fd57e27f1420d5fc9d114c595f02b4f512b7c92aab230079d9225e2e8285b010679a1933d2a0aa9418de83888da1549e24721540246f0ac0fb1b7c28" },
                { "id", "b6a89f1ac6acf838536ede8915d1d4af70cbf55c9c8be195a1a8e89dfeeb609d936ad30161eec2716ac6e04cde12655d034154b336635041152fd4503fb7feb3" },
                { "is", "9eb6f7a5c3b52c2ca681cece77098d839d1cac744307c12fede398a140fcc869f30051d369414677fa95d09c45b916eadbaf7e084a1fe23cb56f9d2e9582a73b" },
                { "it", "d4d6b60f0c48787ec10019d2dc2a444a8c3c3f8e8f6f743e2d839fcecbb9b735e920751c50e59315f3af4ffe8d217754d748d4c5095ed9af6b16c54bd4de5e9c" },
                { "ja", "4976d386fe1e825e55fb8eb7cc67c622cb5b94485a72fe4b452b235229b44014b3c90f43c83441ee35afba3311b10806bd2764602cc2601b5b11a76e2746ac6b" },
                { "ka", "e9239b05b59b24361bbb9b50ccb390052f38e88831881259a9fae7fe18d6f09b1aa850a3431ae00ad7a8b809c949797978201e6acb97926fa76bb0c41708eb73" },
                { "kab", "ca83a416ced1224cad26069b017457341b55eca2f5bb362653031635e5390ef86d95cba3c963434076ac87614f74fe6c9a537d151ee3e878128785829ba472a0" },
                { "kk", "75e28231df55e018be35806d0f6c19c98ba46a2696a838a9cf27b9daa373b627319413816809d748ece107ab211cafd8298b8eff244b59178a7d1517bcd75d99" },
                { "km", "036189dbc5e9c8aacc1777932d68a9af00315701aac888a79913fd3238704246b25f261503dfe7c0d68afe2765d7a9e24dd0c35da15badc20f9be6d3e94cd9e8" },
                { "kn", "748350c93b408f63891dfb9ace9eccdf9e921210e982d8cd92aa6e132a4b98f407e8507254a519926f2760c89a1aa96b84f05fe243226179de499ad51975decf" },
                { "ko", "715e91fa78af1e7cdded80130e6ad8afb22535082a6a499d7ef4dd3f64ea058f702baf2884fbf34f261b84cdfb7fc3ec54d82d4f5c598da492ac3c85197f8d66" },
                { "lij", "f894689f800370685738b3635e188f45757b4fd7a98ab81dbc43ac1d57540f5b05fbe5ba345bb840b693177bb97e07496e0ffc83a1197aa08b84c0b742c9b467" },
                { "lt", "bb08c22772aad3518d544a59e8bd3fb38ef2e80068f7ed08f174b27a35f28db31d50e7e081059ea435fee8eec8ce366f393f15ef5c8a5adfa17c6e97468a14f7" },
                { "lv", "def4b8d53d166b055ecec65f6dd478a38775077e8d24f5f89d90799752fdfa493c89fb4515dcc2537b3b6d043c5c459f8dcb6de5d66d140188e94dc51c358e33" },
                { "mk", "c728655d9bd57328116c76000a7c7f1b873f952ecb17fb76d389c75c105f937a3797cbeb3c2583807ae6277dda80b96bc3417cfdc09df5b3c55ef228ff49b335" },
                { "mr", "44f3512bbf549fb0518f09cbb32b491fe4f280b107220a7fba466025bd7b932b3b05bcc7be30ed8305a58de2aae1e57ebbf6a0c77ede7e8d72b0971e086c9c19" },
                { "ms", "10d2e0ad9b41f081892e0f178f6492a07371653150b63104615f6b9c03b9d492df81468017fe8810e48f8bcfd6c042f49f5eadb941513c3e3d1ce08ffb86a574" },
                { "my", "9c9c9c1c14a430371a8cd412a465934f725bf8fda68db3551aa74c02bf4b71c5e0f448fe7217347383356e80cc06d56868def7864c92ad2e90b96469b65263f0" },
                { "nb-NO", "2bb209bab4dd24ab537dec98d11c758d6e2b18c59b25053ae7751b6535f01421bb19e4e7cd2e36ff3d6e12f2674ba3ea2f0b7fac87ebddfa7e61f70d0c963183" },
                { "ne-NP", "c95c9dad4b4bdc6429ddbe0e02672f7ec349a3abb56cf77d5a0207411bc1061f06c75a2197cdeca5cf69a3d095e30f19b3eb78faf54ea7170bebc217aebcc3fb" },
                { "nl", "3fc06f80806f936d70d70a74e6668f29ac9bd7d31523801498128b3e1a1c98608ee584c1b25cb5fb1d5caaf0331f73a63cc5d7c9bcc17006fe6ee5ccfe05eb56" },
                { "nn-NO", "f31dc556d550d8889841250569877e12e3b7f15230016458344ff084d8cd58dad25d723b39f266ba39f61101f48f4c77dece9a980f69c6a56c98c429f06cbabf" },
                { "oc", "3efe7b7e0fa49f2779e221b6d43fcbca11ab8b880a837f0124f103a50c9e3a85fb468038e87bd8fa9461cd043b7b56a179c2b686aca2d44731a5104d57f92872" },
                { "pa-IN", "29724a85dccee2d4a6d5c70d3dab8674adf3fce536ef6760305e308b987fcad12f7a7293bc783fa8c480c67a43e0ff2ce38f0c7347645504985a1e5f1b7c9012" },
                { "pl", "7e584d6d1374b1db76a95140b91b1bce1757497c471ad12a081fa4d49ad88c5b970d607d89ddb39fe87f2c44abfc67a78c4fecc08d9cbcc5cc50688af5e30c70" },
                { "pt-BR", "78799e467825c5259d74c3c3de15a8bdc32a1578cc009b6ef0743a06419fe4dab624b3aabc2c89368fee073e0c78f0b0a071f80f197382aa9a7bbe326fecc3a0" },
                { "pt-PT", "088474a759151763c00a1862b57e94d339f5e322fb5e4566cf9af925d0a0e8909b8e68d427d6cd2bfbc63c276cb3051c5a9c3a6f1a66c64ea90fdb5765992d33" },
                { "rm", "43f884d85b18030b5601481f60d39d7f1e51f1dd6a347e4ac86274de20f8142352ffe809a65af2bd1c297570e021284cfe5ae13ecec44dbbda758be4823b1f00" },
                { "ro", "980a3a5dc2614eb5982791f91543ded87f908f61645ce337b74fcf7bd86c737e810fb2a2efe9a8007cae7634fee643bbe3f66b2d9c0e25559891d1b37badfcba" },
                { "ru", "224ec835f1609b14902a72f5e126ea107f8395bd866c517df7aacf38cef1a3d9cf17dfc511ce4d56036259d9ea7fb4af456ff3f0b7bae5bc764fe6ee7c9ee60e" },
                { "sco", "ef68101d395f40448d98b4b1f5ca947fcab57a5a3e3923392ecf1779b7643f2d85753d58f550e273ebea7ded6270bfaa1da5151bc4ac5837a88233115a41ee35" },
                { "si", "1b7cfe3c7eab478196799a65eaf06d6e34e1d03ffe65f4abdf9880ca9f2d6044e57ad7995563cca8315599ce003923f2c7d45f2ca385e99878d09525954d370a" },
                { "sk", "0fda9d327c5e9dcd77fb1fdba4f3b1aaa517025c102a20a72c21d40d18faf92c90a29b90f267df1abdca856c447cb9474b952f449e1fb88990157f56080d7d1f" },
                { "sl", "f88a48261234e821d2dfaab36c6e0c6fed9ef17f9cae259479977f95e223ee93e5e251733202a5a33b8416048414c6f8d20fb86932144add1a18d32f796ff01a" },
                { "son", "46e3be9203615d5960bff454c76e38d18d687e0f3a898fda1d3343cb663d21aa9d9f926337c065a638da415e00343bd7b1b988af72b17ec05c28416bc0671566" },
                { "sq", "f15fbcbd04b138835485daf4fb67858c9cb8a5b2913f3f9a851b39b05c7b2ec253c26fbaf193e7cafbdc395947c8b0190d52b4df0c6fa13321244902c18a118e" },
                { "sr", "0b09c2d1a808e8b4779592d9f46e5f5cccd85480c4ea0906e7c2559bd1ea50cc029c9aa948b533956cb019fa5372a2b1462852185699b03a52b4f01abc01641b" },
                { "sv-SE", "a43f45f14c2a162599997b0bf59aa0d798c5124659ce6bb2bd3128d0cda9ea70726dcd51b0ecbbe37006df1ad33e6c77bc0e153cfba90034cccc1ccfb87f59c4" },
                { "szl", "b4d250050eefeeecd767e0d6d09a9f8cdd312e3431cd42d79a4d43e4e462623750f6766d957312a65f1091bffac5705d2bbea6745f0f9a5832bfdbe7fc74093b" },
                { "ta", "3a6c80418537e98952ac4cc6e73182a854610c1d79c4e686a44c898e84e93b969806bed740f0425e0143fd96e0d5e3de5b63d540cf14762b872ad2ff401d3a5b" },
                { "te", "063368340ca59c2791cfefae98cbcf6fc718b6f275813a32d7abd6b0047fa70bedb4185572d2fab2b3db289de74b74a4c43a100d2155eec0d96ccb4e13dd8d2e" },
                { "th", "6d3d61ce2fe9b7ea250726a2b11585d645a5839bfbbdcbb77f2a83867292f5b50181e5b5e352f318c376b184c3ec26b47201260cb28ebba7fa69868d514a0f4c" },
                { "tl", "77351561e900553bf62e751a0e0714f40f187e3dd143bcb0e6cee55a9d2cd9e67e80e0acd26a33accb8116ced1ac018278ce37ebf1a8fd4f516dc6896befc900" },
                { "tr", "afa0bf29638171422d924aef479b876c95a987adc599e46399d9f2e76030dc44c2b44f610bdd9a71a841c55043fc3a3983cbff37bd6ee6ee950c7ef7d335ae09" },
                { "trs", "bfbccfd156693f8c2887ef5f68c31623b6984f3fa7793ebf4d8dae10fc7eb1e9f45cc22f8227e42473bc093a51fb024ce988e46a80c1174f2fddfc32c76efef5" },
                { "uk", "5797cafe9b97a61d7b6ce36db9a75cd216bdb18ce601d97baeeb2859f963601af41d2d3a402b6d8e3a0e8d7c46436dd2beeed215babc2faf4ce3298e07a571da" },
                { "ur", "bb5991f4960639c07560833fc55a52ab858d52bc8a42213dd76c00ea4f5167789519feb027ca9e9f5d9174f7275db9e4a3bf2b0c64343460eb29ef6460a1f6e9" },
                { "uz", "709a0474b3b94de4a6e4acbfbb78061d2d45b76c6eea4666c3a3ff5c7e76661e29e1d9cb5c6eec256d7d9a79f74bf02479b2dba838f64583c447943c8ef4f0c0" },
                { "vi", "5ef0c4169ee487e34b9a1bc51b36372020c5a2c214dae88596d10a8fbe6fe0265ef7375bdfb8b7dda0a5c4f86552b58c85791efe85c4725ac93600bcc17c0d52" },
                { "xh", "a3c0d61e337cde25a8a475cf6c38b8153fdf44127db67d285f9cb81822fcfb002cbbe5ac1f5d08e7767f3e2bab335ee34018931a0b8ea18d5ada4ddac58f6da2" },
                { "zh-CN", "a9de87f3bcc630f95dd41fd99520805adbfbedb7684a2f73157ab9f5b534f12f486765f12d6b3ebcc22b59527185f7d8492031392744e19e3ebffdfdbd4bd31c" },
                { "zh-TW", "73177df336c0d4ee3b4a24c9d1019691edf617ce0c5e6272851e93cc0aaf64ec49725e506c0f6f60f7d4829cfdf6773aaeba19e7ed350e04bcccfcaf5f74b2b0" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
